using FastEndpoints;
using FastEndpoints.Swagger;
using Music.Backend.Middleware;
using Microsoft.EntityFrameworkCore;
using Music.Repository.EF.DatabaseContexts;

namespace Music.Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddLogging(logging => { logging.AddSeq(); });
        builder.Services.AddDbContext<MusicContext>(opt =>
        {
            var connectionString = builder.Configuration.GetConnectionString("music-thing");
            opt.UseSqlServer(connectionString);
        });

        builder.Services.AddFastEndpoints()
            .SwaggerDocument();

        builder.ConfigureServices();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseGlobalExceptionHandler();
        app.UseLogger();
        app.UseAuthorization();
        app.MapControllers();
        app.UseFastEndpoints()
            .UseSwaggerGen();
        app.Run();
    }
}
