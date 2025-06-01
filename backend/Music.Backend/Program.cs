using FastEndpoints;
using FastEndpoints.Swagger;
using Music.Backend.Middleware;
using Music.Backend.Startup;
using Music.Backend.Startup.ConfigModels;

var builder = WebApplication.CreateBuilder(args);

Music.CommandHandlers.Load.Initialize();
Music.QueryHandlers.Load.Initialize();
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
builder.Services.ConfigureQueryHandlers(types);
builder.Services.ConfigureServices(types);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsConfiguration.ConfigurationName);
app.UseGlobalExceptionHandler();
app.UseLogger();
app.UseAuthorization();
app.MapControllers();
app.UseFastEndpoints()
    .UseSwaggerGen();
app.Run();
