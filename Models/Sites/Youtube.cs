using AudioStreamer.Constants;

namespace AudioStreamer.Models.Sites;

public class Youtube : Site
{
    public override SupportedSites Type => SupportedSites.Youtube;

    public override Task<StreamMetadata> GetMetadataAsync(string url)
    {
        throw new NotImplementedException();
    }

    public override IAsyncEnumerable<AudioChunk> StreamAudioAsync(string url, CancellationToken token)
    {
        //here the scraping will happen with playwright
        throw new NotImplementedException();
    }
}