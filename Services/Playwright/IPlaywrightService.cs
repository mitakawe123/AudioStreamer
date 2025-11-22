namespace AudioStreamer.Services.Playwright;

public interface IPlaywrightService
{
    IAsyncEnumerable<byte[]> StreamResponsesAsync(string url, string urlPathToIntercept);
}