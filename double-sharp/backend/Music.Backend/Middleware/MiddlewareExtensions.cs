namespace Music.Backend.Middleware;

public static class MiddlewareExtensions
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<GlobalExceptionHandler>();
    }

    public static void UseLogger(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<MusicAppLogger>();
    }
}
