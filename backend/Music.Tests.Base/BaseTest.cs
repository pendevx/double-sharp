using Microsoft.EntityFrameworkCore;
using Music.Models.Data.DbContexts;
using Music.Models.Domain;

namespace Music.Tests.Base;

public abstract class BaseTest
{
    protected readonly MusicContext DbContext;

    protected BaseTest()
    {
        var dbOptionsBuilder = new DbContextOptionsBuilder<MusicContext>().UseInMemoryDatabase("MusicThing");
        
        DbContext = new MusicContext(dbOptionsBuilder.Options);
        
        SetupDbContextWithConstants();
    }

    private void SetupDbContextWithConstants()
    {
        var allRoles = Enum.GetNames<RoleName>()
            .Select(rn => new Role { Name = rn.ToString() });
        
        DbContext.Roles.AddRange(allRoles);

        DbContext.SaveChanges();
    }
}
