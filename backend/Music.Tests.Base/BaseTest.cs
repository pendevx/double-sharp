using Microsoft.EntityFrameworkCore;
using Music.EntityFramework;
using Music.Models.Data;

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
        DbContext.Roles.AddRange(EntityFramework.ModelConfigurations.RoleConfiguration.RolesTable);
        DbContext.SaveChanges();
    }
}
