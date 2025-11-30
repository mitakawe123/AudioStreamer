using AudioStreamer.Background;
using AudioStreamer.Models.Sites;
using AudioStreamer.Services.Playwright;
using AudioStreamer.Services.SiteStrategy;
using FastEndpoints;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();
bld.Services
    .AddFastEndpoints()
    .SwaggerDocument();

bld.Services
    .AddSingleton<IStreamJob, StreamJob>()
    .AddScoped<ISiteStrategyFactory, SiteStrategyFactoryFactory>()
    .AddScoped<IPlaywrightService, PlaywrightService>()
    .AddScoped<Youtube>()
    .AddScoped<Nova>();

bld.Services.AddHostedService((sp) => (StreamJob)sp.GetRequiredService<IStreamJob>());

var app = bld.Build();
app
    .UseFastEndpoints()
    .UseSwaggerGen();

app.Run();