using System.Security.Authentication;
using Music.Global.Contracts;
using Music.Repository.EF.DatabaseContexts;

namespace Music.QueryHandlers.Accounts;

public class CheckCurrentAccountHasRoleHandler : IBaseQueryHandler<string, bool>
{
    private readonly MusicContext _dbContext;
    private readonly IAuthContext _authContext;

    public CheckCurrentAccountHasRoleHandler(MusicContext dbContext, IAuthContext authContext)
    {
        _dbContext = dbContext;
        _authContext = authContext;
    }

    public bool Execute(string roleName)
    {
        var account = _authContext.GetAccount();

        if (account is null)
            throw new AuthenticationException();

        var accountHasRole = _dbContext.AccountRoles.Any(ar => ar.AccountId == account.Id && ar.Role!.Name == roleName);

        return accountHasRole;
    }
}
