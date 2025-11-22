using AudioStreamer.Models.Sites;

namespace AudioStreamer.Services.Playwright.SiteStrategy;

public interface ISiteStrategyFactory
{
    Site GetSite(string url);
}