using Music.Models.Data.Utils;

namespace Music.Backend.HttpContextExtensions;

public static class AuthenticationCookie
{
    private const string CookieName = "Authorization";

    public static Guid GetAuthenticationCookie(this HttpRequest ctx) =>
        ctx.GetAuthenticationCookie_Option()
            .Match(cookie => cookie, () => Guid.Empty);

    public static OptionType<Guid> GetAuthenticationCookie_Option(this HttpRequest ctx) =>
        // ctx.Cookies[CookieName]
        ctx.Headers.Authorization.ToString()
            .ToOption()
            .Bind(ParseSessionCookie);

    private static OptionType<Guid> ParseSessionCookie(string reqCookie) =>
        Guid.TryParse(reqCookie, out var cookieGuid) ? Option.Some(cookieGuid) : Option.None<Guid>();

    public static void SetAuthenticationCookie(this HttpResponse ctx, Guid value)
    {
        var config = ctx.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var cookieDomain = config.GetValue<string>("CookieDomain");

        if (value == Guid.Empty)
        {
            ctx.Cookies.Delete(CookieName);
        }
        else
        {
            ctx.Cookies.Append(CookieName, value.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(7),
                Domain = cookieDomain,
            });
        }
    }

    public static void ResetAuthenticationCookie(this HttpResponse ctx)
    {
        ctx.SetAuthenticationCookie(Guid.Empty);
    }
}
