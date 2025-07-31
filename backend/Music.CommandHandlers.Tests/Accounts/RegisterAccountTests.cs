using Music.CommandHandlers.Accounts;
using Music.Commands.Accounts;
using Music.Models.Data;
using Music.Tests.Base;

namespace Music.CommandHandlers.Tests.Accounts;

public class RegisterAccountTests : BaseTest
{
    [Fact]
    public void RegisteredAccount_HasUserRole()
    {
        var handler = new RegisterAccountHandler(DbContext);

        const string username = "testUsername";
        const string password = "testPassword";
        const string displayName = "Test User";

        handler.Execute(new RegisterAccountCommand(username, password, displayName));

        var registeredAccountId = DbContext.Accounts.First(acc => acc.Username == username).Id;

        var hasUserRole = DbContext.Accounts
            .Any(a => a.Id == registeredAccountId && a.Roles.Select(r => r.Name).Contains(nameof(RoleName.User)));

        Assert.True(hasUserRole);
    }
}
