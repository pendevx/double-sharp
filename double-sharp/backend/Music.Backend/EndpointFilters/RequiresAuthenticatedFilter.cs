using Music.Backend.HttpContextExtensions;
using Music.EntityFramework;

namespace Music.Backend.EndpointFilters;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RequiresAuthenticatedAttribute : Attribute;

public class RequiresAuthenticatedFilter : IEndpointFilter
{
    private readonly MusicContext _dbContext;

    public RequiresAuthenticatedFilter(MusicContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var authCookie = context.HttpContext.Request.GetAuthenticationCookie();

        if (authCookie == Guid.Empty)
            return Results.Unauthorized();

        var isActiveSession = _dbContext.Sessions.FirstOrDefault(s => s.Token == authCookie)?.ExpiresOn > DateTime.UtcNow;

        if (!isActiveSession)
            return Results.Unauthorized();

        return await next(context);
    }
}
