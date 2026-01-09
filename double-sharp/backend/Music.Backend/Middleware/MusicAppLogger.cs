namespace Music.Backend.Middleware;

public class MusicAppLogger
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MusicAppLogger> _logger;

    public MusicAppLogger(RequestDelegate next, ILogger<MusicAppLogger> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            _logger.LogInformation("Request received: {HttpMethod} {HttpPath}", context.Request.Method, context.Request.Path);
            await _next(context);
            _logger.LogInformation("Response sent: {HttpResponseStatusCode} {HttpMethod} {HttpPath}", context.Response.StatusCode, context.Request.Method, context.Request.Path);

            string bodyContent = await new StreamReader(context.Request.Body).ReadToEndAsync();
            _logger.LogInformation("{Body}", bodyContent);
        }
        catch (Exception e)
        {
            _logger.LogError("{Error}", e.ToString());
            throw;
        }

        _logger.LogInformation("");
    }
}
