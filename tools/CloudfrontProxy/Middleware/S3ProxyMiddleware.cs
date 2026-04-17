using Amazon.S3;
using Amazon.S3.Model;
using CloudFrontLocalProxy.Configuration;
using Microsoft.Extensions.Options;

namespace CloudFrontLocalProxy.Middleware;

/// <summary>
/// Terminal middleware that proxies validated requests to an S3 bucket.
/// Streams the S3 object back to the client, preserving content type and cache headers.
/// </summary>
public sealed class S3ProxyMiddleware
{
    private readonly ILogger<S3ProxyMiddleware> _logger;

    // RequestDelegate not stored because this is a terminal middleware
    public S3ProxyMiddleware(RequestDelegate next, ILogger<S3ProxyMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAmazonS3 s3Client, IOptions<CloudFrontProxyOptions> options)
    {
        var opts = options.Value;
        var requestPath = context.Request.Path.Value?.TrimStart('/') ?? string.Empty;

        if (string.IsNullOrEmpty(requestPath))
        {
            requestPath = "index.html";
        }

        _logger.LogInformation("Proxying to S3: bucket={Bucket}, key={Key}", opts.S3BucketName, requestPath);

        try
        {
            var request = new GetObjectRequest
            {
                BucketName = opts.S3BucketName,
                Key = requestPath
            };

            // Forward Range header if present
            if (context.Request.Headers.TryGetValue("Range", out var rangeHeader))
            {
                var rangeValue = rangeHeader.ToString();
                // Parse "bytes=start-end" format
                if (rangeValue.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase))
                {
                    var rangeParts = rangeValue["bytes=".Length..].Split('-');
                    if (rangeParts.Length == 2
                        && long.TryParse(rangeParts[0], out var start)
                        && long.TryParse(rangeParts[1], out var end))
                    {
                        request.ByteRange = new ByteRange(start, end);
                    }
                }
            }

            using var response = await s3Client.GetObjectAsync(request, context.RequestAborted);

            context.Response.StatusCode = (int)response.HttpStatusCode;
            context.Response.ContentType = response.Headers.ContentType;

            if (response.Headers.ContentLength > 0)
            {
                context.Response.ContentLength = response.Headers.ContentLength;
            }

            // Forward cache-related headers
            if (!string.IsNullOrEmpty(response.Headers.CacheControl))
            {
                context.Response.Headers.CacheControl = response.Headers.CacheControl;
            }

            if (!string.IsNullOrEmpty(response.Headers["ETag"]))
            {
                context.Response.Headers.ETag = response.Headers["ETag"];
            }

            if (!string.IsNullOrEmpty(response.Headers["Last-Modified"]))
            {
                context.Response.Headers.LastModified = response.Headers["Last-Modified"];
            }

            await response.ResponseStream.CopyToAsync(context.Response.Body, context.RequestAborted);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("S3 object not found: {Key}", requestPath);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Not Found",
                key = requestPath
            });
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "S3 error for key: {Key}", requestPath);
            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "S3 Error",
                message = ex.Message
            });
        }
    }
}
