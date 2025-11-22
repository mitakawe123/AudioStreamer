using AudioStreamer.Constants;
using AudioStreamer.Models.Sites;

namespace AudioStreamer.Services.Playwright.SiteStrategy;

public class SiteStrategyFactoryFactory(IServiceProvider serviceProvider) : ISiteStrategyFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Site GetSite(string url)
    {
        if (url.Contains(nameof(SupportedSites.Youtube), StringComparison.OrdinalIgnoreCase))
            return _serviceProvider.GetRequiredService<Youtube>();
        
        throw new NotImplementedException();
    }
}