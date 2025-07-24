using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace Music.Backend.Startup;

public static class CookiesConfig
{
    public static bool IsEnabled { get; private set; }

    public static async Task SetupYouTubeCookies(this WebApplication app)
    {
        var ssmClient = app.Services.GetRequiredService<IAmazonSimpleSystemsManagement>();
        var logger = app.Services.GetRequiredService<ILogger>();
        var parameterName = app.Services.GetRequiredService<IConfiguration>().GetValue<string>("CookiesParameterName");

        logger.LogInformation("Gonna get cookies!");

        try
        {
            var cookieText = (await ssmClient.GetParameterAsync(new GetParameterRequest { Name = parameterName }))
                .Parameter.Value;

            if (string.IsNullOrWhiteSpace(cookieText))
                return;

            await using var cookiesFile = File.CreateText(Path.Combine(Environment.CurrentDirectory, "cookies.txt"));

            await cookiesFile.WriteAsync(cookieText);
            await cookiesFile.FlushAsync();

            logger.LogInformation("Cookies successfully written locally.");
            Console.WriteLine("Cookies successfully written locally.");
            IsEnabled = true;
        }
        catch (Exception e)
        {
            logger.LogError("{Error}", e);
        }
    }
}
