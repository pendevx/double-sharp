# CloudFront Local Proxy

A lightweight ASP.NET Core proxy that emulates AWS CloudFront's **signed cookie** verification locally. It validates `CloudFront-Policy`, `CloudFront-Signature`, and `CloudFront-Key-Pair-Id` cookies using RSA, then proxies valid requests to an S3-compatible backend (LocalStack, MinIO, or real AWS S3).

## Features

- **Full signed cookie verification** — RSA-SHA1 signature validation (same algorithm as CloudFront)
- **Custom policy support** — expiry (`DateLessThan`), not-before (`DateGreaterThan`), resource wildcards, and IP restrictions (`IpAddress`)
- **S3 reverse proxy** — streams objects from S3/LocalStack with content type, cache headers, and range requests
- **Health endpoint** — `GET /health` bypasses cookie checks
- **Configurable** — all settings via `appsettings.json` or environment variables

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or .NET 8+ with target framework change)
- An S3-compatible backend — [LocalStack](https://github.com/localstack/localstack) (free) or real AWS S3
- An RSA key pair for signing/verifying cookies

## Quick Start

### 1. Generate an RSA key pair (if you don't have one)

```bash
mkdir -p keys
openssl genrsa -out keys/private_key.pem 2048
openssl rsa -in keys/private_key.pem -pubout -out keys/public_key.pem
```

### 2. Start LocalStack (free S3)

```bash
docker run --rm -p 4566:4566 localstack/localstack
```

### 3. Create a test bucket and upload a file

```bash
aws --endpoint-url=http://localhost:4566 s3 mb s3://my-cdn-bucket
echo "Hello from local CDN!" > test.txt
aws --endpoint-url=http://localhost:4566 s3 cp test.txt s3://my-cdn-bucket/test.txt
```

### 4. Configure and run the proxy

Edit `appsettings.json` to match your setup:

```json
{
  "CloudFrontProxy": {
    "PublicKeyPemPath": "./keys/public_key.pem",
    "KeyPairId": "APKAEXAMPLE",
    "S3ServiceUrl": "http://localhost:4566",
    "S3BucketName": "my-cdn-bucket",
    "S3Region": "us-east-1",
    "S3ForcePathStyle": true,
    "S3AccessKeyId": "test",
    "S3SecretAccessKey": "test",
    "EnforceIpRestriction": false
  }
}
```

```bash
dotnet run
```

The proxy listens on `http://localhost:5000` by default (or configure via `ASPNETCORE_URLS`).

### 5. Generate signed cookies and test

Use the AWS SDK to generate signed cookies with your **private** key:

```csharp
using Amazon.CloudFront;

var cookies = AmazonCloudFrontCookieSigner.GetCookiesForCustomPolicy(
    resourceUrlOrPath: "http*://localhost:5000/*",
    privateKeyReader: new StreamReader("keys/private_key.pem"),
    keyPairId: "APKAEXAMPLE",
    expiresOn: DateTime.UtcNow.AddHours(1),
    activeFrom: null,
    ipRange: null
);

// cookies.Policy, cookies.Signature, cookies.KeyPairId contain the cookie values
```

Then make a request with those cookies:

```bash
curl -v http://localhost:5000/test.txt \
  --cookie "CloudFront-Policy=<policy_value>" \
  --cookie "CloudFront-Signature=<signature_value>" \
  --cookie "CloudFront-Key-Pair-Id=APKAEXAMPLE"
```

Without cookies → `403 Forbidden`. With valid cookies → file content from S3.

## Configuration Reference

| Setting               | Description                                                     | Default          |
|-----------------------|-----------------------------------------------------------------|------------------|
| `PublicKeyPemPath`    | Path to the RSA public key PEM file                             | *(required)*     |
| `KeyPairId`           | Expected CloudFront Key Pair ID                                 | *(required)*     |
| `S3ServiceUrl`        | S3 endpoint URL                                                 | *(required)*     |
| `S3BucketName`        | S3 bucket to proxy from                                         | *(required)*     |
| `S3Region`            | AWS region for S3                                               | `us-east-1`      |
| `S3ForcePathStyle`    | Use path-style S3 addressing (required for LocalStack)          | `true`           |
| `S3AccessKeyId`       | AWS Access Key (use `test` for LocalStack)                      | `test`           |
| `S3SecretAccessKey`   | AWS Secret Key (use `test` for LocalStack)                      | `test`           |
| `EnforceIpRestriction`| Whether to enforce IP restrictions from the policy              | `true`           |

## How It Works

1. **Request arrives** at the proxy
2. `SignedCookieMiddleware` extracts the three `CloudFront-*` cookies
3. `SignedCookieVerifier` performs:
   - Key Pair ID validation
   - CloudFront-style Base64 decoding (URL-safe: `-` `_` `~` instead of `+` `/` `=`)
   - RSA-SHA1 signature verification against the public key
   - Policy JSON parsing and condition evaluation (expiry, not-before, resource, IP)
4. If valid → `S3ProxyMiddleware` fetches the object from S3 and streams it back
5. If invalid → `403 Forbidden` with a JSON error body

## Usage with Docker Compose

```yaml
services:
  localstack:
    image: localstack/localstack
    ports:
      - "4566:4566"

  cdn-proxy:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_URLS=http://+:8080
      - CloudFrontProxy__S3ServiceUrl=http://localstack:4566
      - CloudFrontProxy__PublicKeyPemPath=/app/keys/public_key.pem
      - CloudFrontProxy__KeyPairId=APKAEXAMPLE
      - CloudFrontProxy__S3BucketName=my-cdn-bucket
    volumes:
      - ./keys:/app/keys:ro
```
