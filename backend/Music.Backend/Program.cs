using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Music.Backend.Middleware;
using Microsoft.EntityFrameworkCore;
using Music.Models.Data.DbContexts;

namespace Music.Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        const string corsAllowProd = "allow-prod";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsAllowProd, policy =>
            {
                policy.WithOrigins("https://music.pendevx.com");
                policy.WithMethods("OPTIONS", "HEAD", "GET", "POST", "PUT", "PATCH", "DELETE");
                policy.WithHeaders("*");
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddLogging(logging => { logging.AddSeq(); });
        builder.Services.AddDbContext<MusicContext>(opt =>
        {
            const string connectionStringKey = "DOUBLESHARP_DB_CONNECTION_STRING";
            var connectionString = builder.Configuration.GetConnectionString("music-thing") ??
                                   Environment.GetEnvironmentVariable(connectionStringKey) ??
                                   throw new InvalidOperationException($"Please specify the connection string in appsettings.json or in an environment variable called {connectionStringKey}.");
            opt.UseNpgsql(connectionString);
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

        app.UseCors(corsAllowProd);
        app.UseGlobalExceptionHandler();
        app.UseLogger();
        app.UseAuthorization();
        app.MapControllers();
        app.UseFastEndpoints()
            .UseSwaggerGen();
        app.Run();
    }
}
