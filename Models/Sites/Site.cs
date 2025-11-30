using AudioStreamer.Constants;

namespace AudioStreamer.Models.Sites;

public abstract class Site
{
    public abstract string UrlPathToIntercept { get; }
    
    public abstract SupportedSites Type { get; }

    public abstract Task NavigateAsync(string url);
    
    public abstract Task RemoveObstaclesAsync(CancellationToken token);
    
    public abstract IAsyncEnumerable<AudioChunk> StreamAudioAsync(CancellationToken token);
}