using AudioStreamer.Constants;
using AudioStreamer.Models.Sites;

namespace AudioStreamer.Services.SiteStrategy;

public class SiteStrategyFactoryFactory(IServiceProvider serviceProvider) : ISiteStrategyFactory
{ 
    private readonly Dictionary<string, Type> _strategies = new()
    {
        { nameof(SupportedSites.Youtube), typeof(Youtube) },
        { nameof(SupportedSites.Nova), typeof(Nova) }
    };
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Site GetSite(string url)
    {
        foreach (var (key, type) in _strategies)
        {
            if (url.Contains(key, StringComparison.OrdinalIgnoreCase))
                return (Site)_serviceProvider.GetRequiredService(type);
        }

        throw new NotImplementedException($"No strategy for {url}");
    }
}