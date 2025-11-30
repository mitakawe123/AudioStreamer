namespace AudioStreamer.Services.Playwright;

public interface IPlaywrightService
{
    Task NavigateAsync(string url);

    Task ClickButtonAsync(string buttonText);
    
    IAsyncEnumerable<byte[]> StreamResponsesAsync(string urlPathToIntercept);

    ValueTask CloseAsync();
}