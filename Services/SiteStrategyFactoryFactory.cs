using AudioStreamer.Constants;
using AudioStreamer.Models.Sites;

namespace AudioStreamer.Services;

public class SiteStrategyFactoryFactory : ISiteStrategyFactory
{
    public Site GetSite(string url)
    {
        if (url.Contains(nameof(SupportedSites.Youtube), StringComparison.OrdinalIgnoreCase))
            return new Youtube();
        
        throw new NotImplementedException();
    }
}