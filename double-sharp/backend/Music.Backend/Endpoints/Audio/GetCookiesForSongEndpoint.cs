using Amazon.CloudFront;
using Music.Backend.EndpointConfigurations;

namespace Music.Backend.Endpoints.Audio;

public record GetCookiesForSongRequest(int Id);

[HttpGet("/music/download-hls/{id}")]
[NoBody]
public class GetCookiesForSongEndpoint : Ep.Req<GetCookiesForSongRequest>.NoRes
{
    public override Task HandleAsync(GetCookiesForSongRequest req, CancellationToken ct)
    {
        var expiration = DateTime.UtcNow.AddMinutes(5);

        var cookies = AmazonCloudFrontCookieSigner.GetCookiesForCustomPolicy(
            $"/Songs/{req.Id}/*",
            new StreamReader("./private_key.pem"), "APKAEXAMPLE",
            expiration,
            DateTime.UtcNow,
            "0.0.0.0/0");

        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var cookieDomain = config.GetValue<string>("CookieDomain");

        HttpContext.Response.Cookies.Append(cookies.Signature.Key, cookies.Signature.Value, new CookieOptions
        {
            SameSite = SameSiteMode.Strict,
            Secure = true,
            HttpOnly = true,
            Domain = cookieDomain,
            MaxAge = TimeSpan.FromMinutes(5),
        });

        HttpContext.Response.Cookies.Append(cookies.KeyPairId.Key, cookies.KeyPairId.Value, new CookieOptions
        {
            SameSite = SameSiteMode.Strict,
            Secure = true,
            HttpOnly = true,
            Domain = cookieDomain,
            MaxAge = TimeSpan.FromMinutes(5),
        });

        HttpContext.Response.Cookies.Append(cookies.Policy.Key, cookies.Policy.Value, new CookieOptions
        {
            SameSite = SameSiteMode.Strict,
            Secure = true,
            HttpOnly = true,
            Domain = cookieDomain,
            MaxAge = TimeSpan.FromMinutes(5),
        });

        return Task.CompletedTask;
    }
}
