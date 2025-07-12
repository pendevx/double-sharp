using Microsoft.EntityFrameworkCore;
using Music.Backend.HttpContextExtensions;
using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;

namespace Music.Backend.Global.Impl;

public class WebAuthContext : IAuthContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MusicContext _dbContext;

    public WebAuthContext(IHttpContextAccessor httpContextAccessor, MusicContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    public Account? GetAccount()
    {
        var authCookie = _httpContextAccessor.HttpContext?.Request.GetAuthenticationCookie();

        if (authCookie is null)
            return null;

        var account = _dbContext.Sessions
            .Include(s => s.Account)
            .FirstOrDefault(s => s.Token == authCookie)
            ?.Account;

        return account;
    }
}
