using System.Buffers;
using System.Runtime.CompilerServices;
using AudioStreamer.Constants;
using AudioStreamer.Services.Playwright;

namespace AudioStreamer.Models.Sites;

public class Youtube(IPlaywrightService playwrightService, ILogger<Youtube> logger) : Site
{
    private const int DefaultBufferSize = 65536; //64KB
    private const string UrlPathToIntercept = "videoplayback";
    
    private readonly IPlaywrightService _playwrightService = playwrightService;
    private readonly ILogger<Youtube> _logger = logger;

    public override SupportedSites Type => SupportedSites.Youtube;

    public override async IAsyncEnumerable<AudioChunk> StreamAudioAsync(string url, [EnumeratorCancellation] CancellationToken token)
    { 
        var streamer = _playwrightService.StreamResponsesAsync(url, UrlPathToIntercept);
        await foreach (var audioChunk in streamer)
        {
            // Process chunk: send to transcription / TTS pipeline
            _logger.LogInformation("Received chunk {audioChunk} bytes", audioChunk);
            yield return new AudioChunk(audioChunk);
        }
    }
}