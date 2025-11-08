using System.Threading.Channels;
using AudioStreamer.Models;

namespace AudioStreamer.Background;

public class StreamJob(ILogger<StreamJob> logger) : BackgroundService, IStreamJob
{    
    private readonly Channel<StreamJobDto> _channel = Channel.CreateUnbounded<StreamJobDto>();
    private readonly ILogger<StreamJob>  _logger = logger;

    public async ValueTask EnqueueAsync(StreamJobDto streamStartRequest)
    {
        await _channel.Writer.WriteAsync(streamStartRequest);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background worker started.");

        await foreach (var item in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Processing item: {Item}", item);
                _logger.LogInformation("Finished processing: {Item}", item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing item: {Item}", item);
            }
        }

        _logger.LogInformation("Background worker stopped.");    }
}