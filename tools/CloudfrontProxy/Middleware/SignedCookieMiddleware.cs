using CloudFrontLocalProxy.Services;

namespace CloudFrontLocalProxy.Middleware;

/// <summary>
/// ASP.NET Core middleware that validates CloudFront signed cookies on every request.
/// Returns 403 if cookies are missing or invalid; passes through to the S3 proxy if valid.
/// </summary>
public sealed class SignedCookieMiddleware
{
    private const string CookiePolicy = "CloudFront-Policy";
    private const string CookieSignature = "CloudFront-Signature";
    private const string CookieKeyPairId = "CloudFront-Key-Pair-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<SignedCookieMiddleware> _logger;

    public SignedCookieMiddleware(RequestDelegate next, ILogger<SignedCookieMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, SignedCookieVerifier verifier)
    {
        var path = context.Request.Path.Value ?? "/";

        // Skip health check endpoint
        if (path.Equals("/health", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        context.Request.Cookies.TryGetValue(CookiePolicy, out var policy);
        context.Request.Cookies.TryGetValue(CookieSignature, out var signature);
        context.Request.Cookies.TryGetValue(CookieKeyPairId, out var keyPairId);

        var clientIp = context.Connection.RemoteIpAddress?.ToString();

        var result = verifier.Verify(policy, signature, keyPairId, path, clientIp);

        if (!result.IsValid)
        {
            _logger.LogWarning("Request denied for {Path}: {Error}", path, result.ErrorMessage);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Access Denied",
                reason = result.ErrorMessage
            });
            return;
        }

        _logger.LogInformation("Request authorized for {Path}", path);
        await _next(context);
    }
}
