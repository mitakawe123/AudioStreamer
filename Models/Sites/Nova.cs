using System.Runtime.CompilerServices;
using AudioStreamer.Constants;
using AudioStreamer.Services.Playwright;

namespace AudioStreamer.Models.Sites;

public sealed class Nova(IPlaywrightService playwrightService, ILogger<Nova> logger) : Site
{
    public override string UrlPathToIntercept => "novaplay-vod";
    
    public override SupportedSites Type => SupportedSites.Nova;

    private readonly IPlaywrightService _playwrightService = playwrightService;
    private readonly ILogger<Nova> _logger = logger;

    public override async Task NavigateAsync(string url)
    {
        await _playwrightService.NavigateAsync(url);
    }

    public override Task RemoveObstaclesAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }
    
    public override async IAsyncEnumerable<AudioChunk> StreamAudioAsync([EnumeratorCancellation] CancellationToken token)
    {
        var streamer = _playwrightService.StreamResponsesAsync(UrlPathToIntercept);
        await foreach (var audioChunk in streamer)
        {
            // Process chunk: send to transcription / TTS pipeline
            _logger.LogInformation("Received chunk {audioChunk} bytes", audioChunk);
            yield return new AudioChunk(audioChunk);
        }    
    }
}