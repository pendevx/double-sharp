using Microsoft.EntityFrameworkCore;
using Music.Backend.HttpContextExtensions;
using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;

namespace Music.Backend.Global.Impl;

public static class Requirements
{
    public static RequiresPermission RequiresPermission(MusicContext dbContext, IHttpContextAccessor httpContextAccessor) =>
        (params RoleName[] roles) =>
        {
            if (roles.Length == 0)
                return true;

            var httpContext = httpContextAccessor.HttpContext;

            var userRoles = dbContext.Accounts.AsNoTracking()
                .Where(a => a.Sessions.Any(s => s.Token == httpContext!.Request.GetAuthenticationCookie()))
                .Select(a => a.Roles.SelectMany(r => r.Name));

            return roles.All(role => userRoles.Contains(role.ToString()));
        };
}
