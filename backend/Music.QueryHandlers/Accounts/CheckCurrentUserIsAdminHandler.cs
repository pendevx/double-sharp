using Music.Global.Contracts;
using Music.Models.Domain;
using Music.Services.Validators;

namespace Music.QueryHandlers.Accounts;

public class CheckCurrentUserIsAdminHandler : IBaseQueryHandler<bool>
{
    private readonly IAuthContext _authContext;
    private readonly AccountHasRoleValidator _accountHasRoleValidator;

    public CheckCurrentUserIsAdminHandler(
        IAuthContext authContext,
        AccountHasRoleValidator accountHasRoleValidator)
    {
        _authContext = authContext;
        _accountHasRoleValidator = accountHasRoleValidator;
    }

    public bool Execute()
    {
        var account = _authContext.GetAccount();

        if (account is null)
            return false;

        var accountHasRole = _accountHasRoleValidator.Verify(account.Id, RoleName.Admin);

        return accountHasRole;
    }
}
