using Music.Backend.HttpContextExtensions;
using Music.EntityFramework;
using Music.Global.Contracts;
using Music.Models.Data;
using Music.Models.Data.Utils;

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

    public Account? GetAccount() =>
        GetAccount_Option().Match(x => x, Account? () => null);

    public OptionType<Account> GetAccount_Option() =>
        _httpContextAccessor.HttpContext
            .ToOption()
            .Bind(ctx => ctx.Request.GetAuthenticationCookie_Option())
            .Bind(TryGetAccountFromSession);

    private OptionType<Account> TryGetAccountFromSession(Guid cookie) =>
        _dbContext.Accounts.FirstOrDefault(a => a.Sessions.Any(s => s.Token == cookie)).ToOption();
}
