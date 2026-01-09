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

        var success = handler.Execute(new RegisterAccountCommand(username, password, displayName));

        Assert.True(success);

        var registeredAccount = DbContext.Accounts.First(acc => acc.Username == username);
        var hasUserRole = registeredAccount.HasAllRoles(RoleName.User);

        Assert.True(hasUserRole);
    }
}
