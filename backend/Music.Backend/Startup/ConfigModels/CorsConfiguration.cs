namespace Music.Backend.Startup.ConfigModels;

public record CorsConfiguration(string Origin, string[] AllowedMethods, string[] AllowedHeaders)
{
    public const string ConfigurationName = "AllowedCorsOrigins";
}

public static class CorsExtensions
{
    public static IEnumerable<CorsConfiguration> GetCorsConfiguration(this ConfigurationManager config)
    {
        return config.GetSection(CorsConfiguration.ConfigurationName).Get<CorsConfiguration[]>() ??
               Enumerable.Empty<CorsConfiguration>();
    }
}
