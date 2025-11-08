using AudioStreamer.Background;
using AudioStreamer.Constants;
using AudioStreamer.Models;
using FastEndpoints;

namespace AudioStreamer.Endpoints;

public record StreamStartRequest(string Url, string Language);

public class StartStreamEndpoint(IStreamJob streamJob) : Endpoint<StreamStartRequest, string>
{
    private readonly string[] _supportedLanguages = ["en", "bg"]; //hardcoded for now make it config later
    private readonly IStreamJob _streamJob = streamJob;

    public override void Configure()
    {
        Post("/api/stream/start");
        AllowAnonymous();
    }

    public override Task OnBeforeHandleAsync(StreamStartRequest req, CancellationToken ct)
    {
        if (!_supportedLanguages.Contains(req.Language))
        {
            AddError($"Language '{req.Language}' is not supported.");
            ThrowIfAnyErrors();
        }
        
        return Task.CompletedTask;
    }

    public override async Task<string> ExecuteAsync(StreamStartRequest req, CancellationToken ct)
    {
        var dto = new StreamJobDto(
            JobId: Guid.NewGuid(),
            Language: req.Language,
            Url: req.Url);
        
        await _streamJob.EnqueueAsync(dto);
        return $"enqueued for processing with status: {nameof(QueueStatus.Accepted)} and id: {dto.JobId}";
    }
}
