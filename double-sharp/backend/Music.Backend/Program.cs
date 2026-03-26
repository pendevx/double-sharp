global using FastEndpoints;
global using Music.Backend.EndpointFilters;
using System.Text.Json.Serialization;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Mvc;
using Music.Backend.EndpointConfigurations;
using Music.Backend.Middleware;
using Music.Backend.Startup;
using Music.CommandHandlers;
using YoutubeDLSharp;

var builder = WebApplication.CreateBuilder(args);

Load.Initialize();
Music.Services.Load.Initialize();

var assemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => a.FullName?.StartsWith("Music.") ?? false);

var types = assemblies.SelectMany(s => s.GetTypes()).ToArray();

builder.ConfigureCors();
builder.ConfigureDataSources();

builder.Services.AddControllers();
builder.Services.ConfigureWeb();
builder.Services.ConfigureAws();
builder.Services.ConfigureLogging();
builder.Services.ConfigureCommandHandlers(types);
builder.Services.ConfigureServices(types);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await Utils.DownloadYtDlp();
    await Utils.DownloadFFmpeg();
}

await app.SetupYouTubeCookies();

app.UseCors(CorsConfiguration.ConfigurationName);
app.UseGlobalExceptionHandler();
app.UseLogger();
app.UseAuthorization();
app.MapControllers();
app.UseFastEndpoints(config => config.Endpoints.Configurator = ep =>
    {
        ep.AllowAnonymous();

        if (ep.EndpointType.GetCustomAttributes(typeof(RequiresAuthenticatedAttribute), false).Any())
            ep.Options(o => o.AddEndpointFilter<RequiresAuthenticatedFilter>());

        if (ep.EndpointType.GetCustomAttributes(typeof(NoBodyAttribute), false).Any())
            ep.Description(o => o.ClearDefaultAccepts());
    })
    .UseSwaggerGen();
app.Run();
