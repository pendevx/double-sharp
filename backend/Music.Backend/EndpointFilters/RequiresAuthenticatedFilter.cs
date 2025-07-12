using Music.Backend.HttpContextExtensions;
using Music.QueryHandlers.Accounts;

namespace Music.Backend.EndpointFilters;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RequiresAuthenticatedAttribute : Attribute;

public class RequiresAuthenticatedFilter : IEndpointFilter
{
    private readonly ValidateTokenIsActiveHandler _validateTokenIsActive;

    public RequiresAuthenticatedFilter(ValidateTokenIsActiveHandler validateTokenIsActive)
    {
        _validateTokenIsActive = validateTokenIsActive;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var authCookie = context.HttpContext.Request.GetAuthenticationCookie();

        if (authCookie == Guid.Empty)
            return Results.Unauthorized();

        var isActiveSession = _validateTokenIsActive.Execute(authCookie);

        if (!isActiveSession)
            return Results.Unauthorized();

        return await next(context);
    }
}
