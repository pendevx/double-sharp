using Amazon.S3;
using Amazon.SecurityToken;
using Amazon.SimpleSystemsManagement;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Music.Backend.Global.Impl;
using Music.CommandHandlers;
using Music.Global.Contracts;
using Music.EntityFramework;
using Music.QueryHandlers;
using Music.Services;
using Music.Services.DataAccess.AWS;

namespace Music.Backend.Startup;

public static class DependencyInjectionConfiguration
{
    private static IEnumerable<Type> ExtractConcreteHandlers(Type[] extractFrom, Type[] baseCommandTypes)
    {
        return extractFrom
            .Where(p => p.GetInterfaces()
                .Any(i => baseCommandTypes.Contains(i.IsGenericType ? i.GetGenericTypeDefinition() : i)));
    }

    public static void ConfigureCommandHandlers(this IServiceCollection services, Type[] extractFrom)
    {
        var commandHandlers = ExtractConcreteHandlers(extractFrom,
            [typeof(IBaseCommandHandler), typeof(IBaseCommandHandler<>), typeof(IBaseCommandHandler<,>)]);
        foreach (var handler in commandHandlers)
            services.AddScoped(handler);
    }

    public static void ConfigureQueryHandlers(this IServiceCollection services, Type[] extractFrom)
    {
        var queryHandlers =
            ExtractConcreteHandlers(extractFrom, [typeof(IBaseQueryHandler<>), typeof(IBaseQueryHandler<,>)]);
        foreach (var handler in queryHandlers)
            services.AddScoped(handler);
    }

    public static void ConfigureServices(this IServiceCollection services, Type[] extractFrom)
    {
        var generalServices = extractFrom.Where(s => s.Namespace is not null &&
                                              s.Namespace.StartsWith("Music.Services") &&
                                              s is { IsAbstract: false, IsSealed: false });
        foreach (var service in generalServices)
            services.AddScoped(service);
    }

    public static void ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            foreach (var corsConfig in builder.Configuration.GetCorsConfiguration())
            {
                options.AddPolicy(CorsConfiguration.ConfigurationName, policy =>
                {
                    policy.WithOrigins(corsConfig.Origin);
                    policy.WithMethods(corsConfig.AllowedMethods);
                    policy.WithHeaders(corsConfig.AllowedHeaders);
                    policy.AllowCredentials();
                });
            }
        });
    }

    public static void ConfigureDataSources(this WebApplicationBuilder builder)
    {
        const string connectionStringAppSettingsKey = "music-thing";
        const string connectionStringEnvVarKey = "DOUBLESHARP_DB_CONNECTION_STRING";

        var connectionString = builder.Configuration.GetConnectionString(connectionStringAppSettingsKey) ??
                               Environment.GetEnvironmentVariable(connectionStringEnvVarKey) ??
                               throw new InvalidOperationException(
                                   $"Specify the connection string in the configuration (key '{connectionStringAppSettingsKey}') " +
                                   $"or in an environment variable (name '{connectionStringEnvVarKey}').");

        builder.Services.AddDbContext<MusicContext>(opt => { opt.UseNpgsql(connectionString); });

        builder.Services.AddScoped<SongsRepository>();

        builder.Services.AddDelegate<GetSongPath>(AwsFunctions.GetSongPath, ServiceLifetime.Scoped);
        builder.Services.AddDelegate<CreateSongRequestPath>(AwsFunctions.CreateSongRequestPath, ServiceLifetime.Scoped);
        builder.Services.AddDelegate<GetBucketName>(AwsFunctions.GetBucketName, ServiceLifetime.Singleton);
        builder.Services.AddDelegate<GetEnvironment>(AppEnvironment.GetEnvironment, ServiceLifetime.Singleton);
        builder.Services.AddDelegate<RequiresPermission>(Requirements.RequiresPermission, ServiceLifetime.Scoped);
    }

    public static void ConfigureAws(this IServiceCollection services)
    {
        services
            .AddAWSService<IAmazonS3>()
            .AddAWSService<IAmazonSecurityTokenService>()
            .AddAWSService<IAmazonSimpleSystemsManagement>()
            .AddSingleton<AwsEnvironment>();
    }

    public static void ConfigureWeb(this IServiceCollection services)
    {
        services.AddControllers();

        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddFastEndpoints()
            .SwaggerDocument()
            .AddHttpContextAccessor();

        services.AddScoped<IAuthContext, WebAuthContext>();
    }

    public static void ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging(logging =>
        {
            logging.AddSeq();
            logging.AddConsole();
        });
    }
}
