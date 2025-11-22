using AudioStreamer.Background;
using AudioStreamer.Models.Sites;
using AudioStreamer.Services;
using AudioStreamer.Services.Playwright;
using AudioStreamer.Services.Playwright.SiteStrategy;
using FastEndpoints;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();
bld.Services
    .AddFastEndpoints()
    .SwaggerDocument();

bld.Services.AddSingleton<IStreamJob, StreamJob>();
bld.Services.AddHostedService(sp => (StreamJob)sp.GetRequiredService<IStreamJob>());

bld.Services
    .AddSingleton<ISiteStrategyFactory, SiteStrategyFactoryFactory>()
    .AddSingleton<Youtube>()
    .AddSingleton<IPlaywrightService, PlaywrightService>();

var app = bld.Build();
app
    .UseFastEndpoints()
    .UseSwaggerGen();

app.Run();