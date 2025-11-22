using AudioStreamer.Models.Sites;

namespace AudioStreamer.Services;

public interface ISiteStrategyFactory
{
    Site GetSite(string url);
}