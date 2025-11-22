using System.Threading.Channels;
using Microsoft.Playwright;

namespace AudioStreamer.Services.Playwright;

public class PlaywrightService : IPlaywrightService, IAsyncDisposable
{
    private readonly IPlaywright _playwright;
    private readonly IBrowser _browser;
    private readonly ILogger<PlaywrightService> _logger;

    public PlaywrightService(ILogger<PlaywrightService> logger)
    {
        _logger = logger;
        _playwright = Microsoft.Playwright.Playwright.CreateAsync().Result;
        _browser = _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        }).Result;
    }
    
    public async IAsyncEnumerable<byte[]> StreamResponsesAsync(string url, string urlPathToIntercept)
    {
        var page = await _browser.NewPageAsync();
        var channel = Channel.CreateUnbounded<byte[]>();

        await page.RouteAsync("**/*", async route =>
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

        await page.GotoAsync(url);

        await foreach (var chunk in channel.Reader.ReadAllAsync())
            yield return chunk;

        await page.CloseAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }
}