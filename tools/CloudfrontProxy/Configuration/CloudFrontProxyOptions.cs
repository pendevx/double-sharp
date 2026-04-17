namespace CloudFrontLocalProxy.Configuration;

/// <summary>
/// Configuration for the CloudFront local proxy.
/// </summary>
public sealed class CloudFrontProxyOptions
{
    public const string SectionName = "CloudFrontProxy";

    /// <summary>
    /// Path to the CloudFront RSA public key PEM file used to verify signed cookies.
    /// </summary>
    public required string PublicKeyPemPath { get; set; }

    /// <summary>
    /// The CloudFront Key Pair ID that must match the cookie value.
    /// </summary>
    public required string KeyPairId { get; set; }

    /// <summary>
    /// The S3 endpoint to proxy requests to (e.g. "http://localhost:4566" for LocalStack,
    /// or "https://s3.amazonaws.com" for real AWS).
    /// </summary>
    public required string S3ServiceUrl { get; set; }

    /// <summary>
    /// The S3 bucket name to serve content from.
    /// </summary>
    public required string S3BucketName { get; set; }

    /// <summary>
    /// AWS region for S3 (default: us-east-1).
    /// </summary>
    public string S3Region { get; set; } = "us-east-1";

    /// <summary>
    /// Whether to force path-style addressing for S3 (required for LocalStack). Default: true.
    /// </summary>
    public bool S3ForcePathStyle { get; set; } = true;

    /// <summary>
    /// AWS Access Key ID for S3. Use "test" for LocalStack.
    /// </summary>
    public string S3AccessKeyId { get; set; } = "test";

    /// <summary>
    /// AWS Secret Access Key for S3. Use "test" for LocalStack.
    /// </summary>
    public string S3SecretAccessKey { get; set; } = "test";

    /// <summary>
    /// Port the proxy listens on. Default: 8080.
    /// </summary>
    public int Port { get; set; } = 8080;

    /// <summary>
    /// Whether to enforce IP address restrictions from the policy. Default: true.
    /// </summary>
    public bool EnforceIpRestriction { get; set; } = true;
}
