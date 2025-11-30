using System.Threading.Channels;
using AudioStreamer.Models;
using AudioStreamer.Services.SiteStrategy;

namespace AudioStreamer.Background;

public class StreamJob(IServiceProvider serviceProvider, ILogger<StreamJob> logger) : BackgroundService, IStreamJob
{    
    private readonly Channel<StreamJobDto> _channel = Channel.CreateUnbounded<StreamJobDto>();
    private readonly IServiceProvider _serviceProvider = serviceProvider;
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
            using var scope = _serviceProvider.CreateScope();

            var strategyFactory = scope.ServiceProvider.GetRequiredService<ISiteStrategyFactory>();
            var site = strategyFactory.GetSite(item.Url);

            try
            {
                _logger.LogInformation("Processing item: {Item}", item);

                await site.NavigateAsync(item.Url);
                await site.RemoveObstaclesAsync(stoppingToken);

                await foreach (var chunk in site.StreamAudioAsync(stoppingToken))
                {
                    // STT, TTS, etc.
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing item: {Item}", item);
            }
        }

        _logger.LogInformation("Background worker stopped.");    
    }
}