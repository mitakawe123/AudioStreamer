using AudioStreamer.Background;
using AudioStreamer.Services;
using FastEndpoints;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();
bld.Services
    .AddFastEndpoints()
    .SwaggerDocument();

bld.Services.AddSingleton<IStreamJob, StreamJob>();
bld.Services.AddHostedService(sp => (StreamJob)sp.GetRequiredService<IStreamJob>());

bld.Services.AddScoped<ISiteStrategyFactory, SiteStrategyFactoryFactory>();

var app = bld.Build();
app
    .UseFastEndpoints()
    .UseSwaggerGen();

app.Run();