using AudioStreamer.Endpoints;
using AudioStreamer.Models;

namespace AudioStreamer.Background;

public interface IStreamJob
{
    ValueTask EnqueueAsync(StreamJobDto streamStartRequest);
}