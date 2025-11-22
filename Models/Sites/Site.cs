using AudioStreamer.Constants;

namespace AudioStreamer.Models.Sites;

public abstract class Site
{
    public abstract SupportedSites Type { get; }
    
    public abstract IAsyncEnumerable<AudioChunk> StreamAudioAsync(string url, CancellationToken token);
}