namespace Music.Backend.Middleware;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            await context.Response.WriteAsync("An unhandled error occurred.");
            await context.Response.BodyWriter.CompleteAsync();
        }
    }
}
