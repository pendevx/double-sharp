using Amazon.S3;
using CloudFrontLocalProxy.Configuration;
using CloudFrontLocalProxy.Middleware;
using CloudFrontLocalProxy.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind configuration
builder.Services.Configure<CloudFrontProxyOptions>(
    builder.Configuration.GetSection(CloudFrontProxyOptions.SectionName));

// Register the signed cookie verifier as a singleton
builder.Services.AddSingleton<SignedCookieVerifier>();

// Register the AWS S3 client
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var options = builder.Configuration
        .GetSection(CloudFrontProxyOptions.SectionName)
        .Get<CloudFrontProxyOptions>()!;

    var config = new AmazonS3Config
    {
        ServiceURL = options.S3ServiceUrl,
        ForcePathStyle = options.S3ForcePathStyle,
        AuthenticationRegion = options.S3Region
    };

    return new AmazonS3Client(options.S3AccessKeyId, options.S3SecretAccessKey, config);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        policy.WithOrigins(["http://localhost:5173"]);
        policy.AllowAnyHeader().AllowAnyMethod();
        policy.AllowCredentials();
    });
});

var app = builder.Build();

// Health check (bypasses cookie verification)
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.UseCors("cors");

// Cookie verification middleware → S3 proxy
app.UseMiddleware<SignedCookieMiddleware>();
app.UseMiddleware<S3ProxyMiddleware>();

app.Run();
