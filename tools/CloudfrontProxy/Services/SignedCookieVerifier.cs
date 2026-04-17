using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CloudFrontLocalProxy.Configuration;
using Microsoft.Extensions.Options;

namespace CloudFrontLocalProxy.Services;

/// <summary>
/// Verifies CloudFront signed cookies by validating the RSA signature against the policy.
/// </summary>
public sealed class SignedCookieVerifier
{
    private readonly RSA _publicKey;
    private readonly string _expectedKeyPairId;
    private readonly bool _enforceIpRestriction;
    private readonly ILogger<SignedCookieVerifier> _logger;

    public SignedCookieVerifier(IOptions<CloudFrontProxyOptions> options, ILogger<SignedCookieVerifier> logger)
    {
        _logger = logger;
        var opts = options.Value;
        _expectedKeyPairId = opts.KeyPairId;
        _enforceIpRestriction = opts.EnforceIpRestriction;

        var pemText = File.ReadAllText(opts.PublicKeyPemPath);
        _publicKey = RSA.Create();
        _publicKey.ImportFromPem(pemText.AsSpan());

        _logger.LogInformation("Loaded CloudFront public key for Key Pair ID: {KeyPairId}", _expectedKeyPairId);
    }

    /// <summary>
    /// Verifies the three CloudFront signed cookies and returns the parsed policy if valid.
    /// </summary>
    public VerificationResult Verify(string? policyBase64, string? signatureBase64, string? keyPairId, string requestPath, string? clientIp)
    {
        if (string.IsNullOrEmpty(policyBase64) || string.IsNullOrEmpty(signatureBase64) || string.IsNullOrEmpty(keyPairId))
        {
            _logger.LogWarning("Missing one or more signed cookies");
            return VerificationResult.Fail("Missing signed cookies");
        }

        if (keyPairId != _expectedKeyPairId)
        {
            _logger.LogWarning("Key Pair ID mismatch: expected {Expected}, got {Actual}", _expectedKeyPairId, keyPairId);
            return VerificationResult.Fail("Key Pair ID mismatch");
        }

        // Decode CloudFront-style base64 (uses -_ instead of +/ and no padding)
        var policyBytes = DecodeCloudFrontBase64(policyBase64);
        var signatureBytes = DecodeCloudFrontBase64(signatureBase64);

        if (policyBytes is null || signatureBytes is null)
        {
            _logger.LogWarning("Failed to decode base64 cookie values");
            return VerificationResult.Fail("Invalid base64 encoding");
        }

        // Verify RSA-SHA1 signature (CloudFront uses RSASSA-PKCS1-v1_5 with SHA-1)
        var signatureValid = _publicKey.VerifyData(policyBytes, signatureBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        if (!signatureValid)
        {
            _logger.LogWarning("RSA signature verification failed");
            return VerificationResult.Fail("Invalid signature");
        }

        _logger.LogDebug("RSA signature verified successfully");

        // Parse and validate the policy JSON
        var policyJson = Encoding.UTF8.GetString(policyBytes);
        CloudFrontPolicy? policy;
        try
        {
            policy = JsonSerializer.Deserialize<CloudFrontPolicy>(policyJson);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse policy JSON");
            return VerificationResult.Fail("Invalid policy JSON");
        }

        if (policy?.Statement is null || policy.Statement.Count == 0)
        {
            return VerificationResult.Fail("Policy has no statements");
        }

        var statement = policy.Statement[0];

        // Check resource pattern
        var resourcePattern = statement.Resource;
        if (!string.IsNullOrEmpty(resourcePattern) && !MatchesResource(requestPath, resourcePattern))
        {
            _logger.LogWarning("Resource mismatch: pattern={Pattern}, request={Request}", resourcePattern, requestPath);
            return VerificationResult.Fail("Resource does not match policy");
        }

        // Check expiry (DateLessThan -> AWS:EpochTime)
        var expiryEpoch = statement.Condition?.DateLessThan?.EpochTime;
        if (expiryEpoch.HasValue)
        {
            var expiryUtc = DateTimeOffset.FromUnixTimeSeconds(expiryEpoch.Value);
            if (DateTimeOffset.UtcNow > expiryUtc)
            {
                _logger.LogWarning("Policy expired at {Expiry}", expiryUtc);
                return VerificationResult.Fail("Policy has expired");
            }
        }

        // Check not-before (DateGreaterThan -> AWS:EpochTime)
        var notBeforeEpoch = statement.Condition?.DateGreaterThan?.EpochTime;
        if (notBeforeEpoch.HasValue)
        {
            var notBeforeUtc = DateTimeOffset.FromUnixTimeSeconds(notBeforeEpoch.Value);
            if (DateTimeOffset.UtcNow < notBeforeUtc)
            {
                _logger.LogWarning("Policy not yet valid until {NotBefore}", notBeforeUtc);
                return VerificationResult.Fail("Policy is not yet valid");
            }
        }

        // Check IP restriction (IpAddress -> AWS:SourceIp)
        if (_enforceIpRestriction)
        {
            var allowedIpCidr = statement.Condition?.IpAddress?.SourceIp;
            if (!string.IsNullOrEmpty(allowedIpCidr) && !string.IsNullOrEmpty(clientIp))
            {
                if (!IpMatchesCidr(clientIp, allowedIpCidr))
                {
                    _logger.LogWarning("IP restriction failed: client={ClientIp}, allowed={Cidr}", clientIp, allowedIpCidr);
                    return VerificationResult.Fail("IP address not allowed");
                }
            }
        }

        _logger.LogInformation("Signed cookie verified for resource: {Resource}", resourcePattern);
        return VerificationResult.Success(policyJson);
    }

    private static byte[]? DecodeCloudFrontBase64(string encoded)
    {
        try
        {
            // CloudFront uses URL-safe base64: replace - with +, ~ with =, _ with /
            var standardBase64 = encoded
                .Replace('-', '+')
                .Replace('_', '=')
                .Replace('~', '/');

            Console.WriteLine(encoded);
            Console.WriteLine(standardBase64);
            Console.WriteLine();

            return Convert.FromBase64String(standardBase64);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static byte[]? DecodeMaybeCloudFrontBase64(string encoded)
    {
        try
        {
            encoded = Uri.UnescapeDataString(encoded);

            // If it contains '~' or '-' → CloudFront encoding
            var isCloudFront = encoded.Contains('~') || encoded.Contains('-');

            if (isCloudFront)
            {
                encoded = encoded
                    .Replace('-', '+')
                    .Replace('_', '/')
                    .Replace('~', '=');
            }

            // Fix padding
            switch (encoded.Length % 4)
            {
                case 2: encoded += "=="; break;
                case 3: encoded += "="; break;
            }

            return Convert.FromBase64String(encoded);
        }
        catch
        {
            return null;
        }
    }

    private static bool MatchesResource(string requestPath, string resourcePattern)
    {
        // CloudFront resource patterns look like: https://example.com/path/* or http*://example.com/*
        // For local proxy we just check if the path portion matches using simple wildcard
        if (resourcePattern.Contains("*"))
        {
            // Extract path portion from the resource URL pattern
            var patternPath = ExtractPathFromResource(resourcePattern);
            if (patternPath == "/*" || patternPath == "*")
            {
                return true;
            }

            // Simple prefix wildcard match: /images/* matches /images/foo.jpg
            var prefix = patternPath.TrimEnd('*');
            return requestPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        return true; // If no wildcard, allow (permissive for local dev)
    }

    private static string ExtractPathFromResource(string resource)
    {
        // Pattern like: http*://example.com/path/*
        // We want just /path/*
        var schemeEnd = resource.IndexOf("://", StringComparison.Ordinal);
        if (schemeEnd < 0)
        {
            return resource;
        }

        var hostStart = schemeEnd + 3;
        var pathStart = resource.IndexOf('/', hostStart);
        return pathStart < 0 ? "/" : resource[pathStart..];
    }

    private static bool IpMatchesCidr(string clientIp, string cidr)
    {
        // Handle CIDR like "1.2.3.4/32" or "0.0.0.0/0"
        var parts = cidr.Split('/');
        if (!System.Net.IPAddress.TryParse(parts[0], out var networkAddress))
        {
            return false;
        }

        if (!System.Net.IPAddress.TryParse(clientIp, out var clientAddress))
        {
            return false;
        }

        if (parts.Length == 1)
        {
            return clientAddress.Equals(networkAddress);
        }

        if (!int.TryParse(parts[1], out var prefixLength))
        {
            return false;
        }

        var networkBytes = networkAddress.GetAddressBytes();
        var clientBytes = clientAddress.GetAddressBytes();

        if (networkBytes.Length != clientBytes.Length)
        {
            return false;
        }

        var fullBytes = prefixLength / 8;
        var remainingBits = prefixLength % 8;

        for (var i = 0; i < fullBytes; i++)
        {
            if (networkBytes[i] != clientBytes[i])
            {
                return false;
            }
        }

        if (remainingBits > 0 && fullBytes < networkBytes.Length)
        {
            var mask = (byte)(0xFF << (8 - remainingBits));
            if ((networkBytes[fullBytes] & mask) != (clientBytes[fullBytes] & mask))
            {
                return false;
            }
        }

        return true;
    }
}

/// <summary>
/// Result of signed cookie verification.
/// </summary>
public sealed class VerificationResult
{
    public bool IsValid { get; private init; }
    public string? ErrorMessage { get; private init; }
    public string? PolicyJson { get; private init; }

    public static VerificationResult Success(string policyJson) => new()
    {
        IsValid = true,
        PolicyJson = policyJson
    };

    public static VerificationResult Fail(string error) => new()
    {
        IsValid = false,
        ErrorMessage = error
    };
}

// CloudFront policy JSON model
// Example: {"Statement":[{"Resource":"http*://example.com/*","Condition":{"DateLessThan":{"AWS:EpochTime":1234567890}}}]}

public sealed class CloudFrontPolicy
{
    [JsonPropertyName("Statement")]
    public List<PolicyStatement>? Statement { get; set; }
}

public sealed class PolicyStatement
{
    [JsonPropertyName("Resource")]
    public string? Resource { get; set; }

    [JsonPropertyName("Condition")]
    public PolicyCondition? Condition { get; set; }
}

public sealed class PolicyCondition
{
    [JsonPropertyName("DateLessThan")]
    public EpochCondition? DateLessThan { get; set; }

    [JsonPropertyName("DateGreaterThan")]
    public EpochCondition? DateGreaterThan { get; set; }

    [JsonPropertyName("IpAddress")]
    public IpCondition? IpAddress { get; set; }
}

public sealed class EpochCondition
{
    [JsonPropertyName("AWS:EpochTime")]
    public long? EpochTime { get; set; }
}

public sealed class IpCondition
{
    [JsonPropertyName("AWS:SourceIp")]
    public string? SourceIp { get; set; }
}
