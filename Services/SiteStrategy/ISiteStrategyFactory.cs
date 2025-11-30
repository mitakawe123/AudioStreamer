using AudioStreamer.Models.Sites;

namespace AudioStreamer.Services.SiteStrategy;

public interface ISiteStrategyFactory
{
    Site GetSite(string url);
}