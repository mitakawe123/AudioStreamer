using System.Threading.Channels;
using AudioStreamer.Models;
using AudioStreamer.Services;
using AudioStreamer.Services.Playwright.SiteStrategy;

namespace AudioStreamer.Background;

public class StreamJob(ISiteStrategyFactory siteStrategyFactory, ILogger<StreamJob> logger) : BackgroundService, IStreamJob
{    
    private readonly Channel<StreamJobDto> _channel = Channel.CreateUnbounded<StreamJobDto>();
    private readonly ISiteStrategyFactory _siteStrategyFactory = siteStrategyFactory;
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
                
                var site = _siteStrategyFactory.GetSite(item.Url);
                await foreach (var chunk in site.StreamAudioAsync(item.Url, stoppingToken))
                {
                    // Push chunk to STT
                    // Push chunk to TTS
                    // Save?
                }
                
                _logger.LogInformation("Finished processing: {Item}", item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing item: {Item}", item);
            }
        }

        _logger.LogInformation("Background worker stopped.");    
    }
}