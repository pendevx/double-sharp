namespace Music.Backend.HttpContextExtensions;

public static class AuthenticationCookie
{
    private const string CookieName = "Authorization";

    public static Guid GetAuthenticationCookie(this HttpRequest ctx)
    {
        var cookie = ctx.Cookies[CookieName];
        return cookie is null ? Guid.Empty : Guid.Parse(cookie);
    }

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
