using System.Text.RegularExpressions;
using System.Threading.Channels;
using Microsoft.Playwright;

namespace AudioStreamer.Services.Playwright;

public sealed class PlaywrightService : IPlaywrightService
{
    private const string BraveExecutablePath = "/usr/bin/brave-browser";
    
    private readonly IPlaywright _playwright;
    private readonly IBrowser _browser;

    private IPage? _page;

    public PlaywrightService()
    {
        _playwright = Microsoft.Playwright.Playwright.CreateAsync().Result;
        _browser = _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            ExecutablePath = BraveExecutablePath,
            Headless = false,
            Args =
            [
                "--start-fullscreen",
                "--start-maximized"
            ]
        }).Result;
    }

    public async Task NavigateAsync(string url)
    {
        if (_page is null || _page.IsClosed)
            _page = await _browser.NewPageAsync();

        await _page.GotoAsync(url);
    }

    public async Task ClickButtonAsync(string buttonText)
    {
        if(_page is null)
            throw new InvalidOperationException("Page is not opened");
        
        var consentButton = _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions {
            NameRegex = new Regex(buttonText, RegexOptions.IgnoreCase)
        });

        await consentButton.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000 // handles slow GDPR dialogs
        });

        await consentButton.ClickAsync();
    }

    public async IAsyncEnumerable<byte[]> StreamResponsesAsync(string urlPathToIntercept)
    {
        if(_page is null)
            throw new InvalidOperationException("Page is not opened");
        
        var channel = Channel.CreateUnbounded<byte[]>();

        await _page.RouteAsync("**/*", async route =>
        {
            var request = route.Request;
            if (request.Url.Contains(urlPathToIntercept, StringComparison.OrdinalIgnoreCase))
            {
                var response = await request.ResponseAsync();
                if (response is not null)
                {
                    var buffer = await response.BodyAsync();
                    await channel.Writer.WriteAsync(buffer);
                }
            }
            await route.ContinueAsync();
        });

        await foreach (var chunk in channel.Reader.ReadAllAsync())
            yield return chunk;
    }
    
    public async ValueTask CloseAsync()
    {
        if(_page is not null || _page?.IsClosed is false)
            await _page.CloseAsync();
        
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }
}