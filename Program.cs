using AudioStreamer.Background;
using FastEndpoints;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();
bld.Services
    .AddFastEndpoints()
    .SwaggerDocument();

bld.Services.AddSingleton<IStreamJob, StreamJob>();
bld.Services.AddHostedService(sp => (StreamJob)sp.GetRequiredService<IStreamJob>());

var app = bld.Build();
app
    .UseFastEndpoints()
    .UseSwaggerGen();

app.Run();