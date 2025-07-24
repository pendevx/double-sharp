namespace Music.Backend.Startup;

public record CorsConfiguration(string Origin, string[] AllowedMethods, string[] AllowedHeaders)
{
    public const string ConfigurationName = "AllowedCorsOrigins";
}

public static class AppConfigExtensions
{
    public static IEnumerable<CorsConfiguration> GetCorsConfiguration(this ConfigurationManager config)
    {
        return config.GetSection(CorsConfiguration.ConfigurationName).Get<CorsConfiguration[]>() ??
               Enumerable.Empty<CorsConfiguration>();
    }
}
