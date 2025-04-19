using Microsoft.EntityFrameworkCore;
using Music.Models.Data;
using MssqlContext = Music.Scripts.DS20.Common.MusicContext;
using PsqlContext = Music.Models.Data.DbContexts.MusicContext;

var path = Path.Combine(Environment.CurrentDirectory, "connectionStrings.txt");

if (!File.Exists(path))
{
    File.Create(path);
    Console.WriteLine($"Populate the file {path} before continuing.");
    Console.WriteLine("The first line should contain the MSSQL connection string, and the second line should contain the PSQL connection string.");
    return;
}

var connectionStrings = File.ReadAllLines(path);

var mssqlOptionsBuilder = new DbContextOptionsBuilder<MssqlContext>();
mssqlOptionsBuilder.UseSqlServer(connectionStrings[0]);

var psqlOptionsBuilder = new DbContextOptionsBuilder<PsqlContext>();
psqlOptionsBuilder.UseNpgsql(connectionStrings[1]);

var mssql = new MssqlContext(mssqlOptionsBuilder.Options);
var psql = new PsqlContext(psqlOptionsBuilder.Options);

ExtractAccounts();
ExtractPermissions();
ExtractRoles();
ExtractSessions();
ExtractSongs();
ExtractSongRequests();

psql.SaveChanges();

ExtractAccountRoles();
ExtractRolePermissions();

psql.SaveChanges();
return;

void ExtractAccounts()
{
    var accounts = mssql.Accounts.ToList();
    psql.Accounts.AddRange(accounts.Select(a => new Account
    {
        Id = a.Id,
        Guid = a.Guid,
        Username = a.Username,
        DisplayName = a.DisplayName,
        SaltedPassword = a.SaltedPassword,
    }));
}

void ExtractAccountRoles()
{
    var accountRoles = mssql.AccountRoles.ToList();
    psql.AccountRoles.AddRange(accountRoles.Select(ar => new AccountRole
    {
        Id = ar.Id,
        AccountId = ar.AccountId,
        RoleId = ar.RoleId,
    }));
}

void ExtractPermissions()
{
    var permissions = mssql.Permissions.ToList();
    psql.Permissions.AddRange(permissions.Select(p => new Permission
    {
        Id = p.Id,
        PermissionName = p.PermissionName,
    }));
}

void ExtractRoles()
{
    var roles = mssql.Roles.ToList();
    psql.Roles.AddRange(roles.Select(r => new Role
    {
        Id = r.Id,
        Name = r.Name,
    }));
}

void ExtractRolePermissions()
{
    var rolePermissions = mssql.RolePermissions.ToList();
    psql.RolePermissions.AddRange(rolePermissions.Select(rp => new RolePermission
    {
        Id = rp.Id,
        PermissionId = rp.PermissionId,
        RoleId = rp.RoleId,
    }));
}

void ExtractSessions()
{
    // Sessions don't need to be extracted because they're short-lived data which can easily be refreshed
}

void ExtractSongs()
{
    var songs = mssql.Songs.Select(s => new
    {
        s.Id,
        s.Name,
        s.MimeType,
        s.Guid,
    }).ToList();
    psql.Songs.AddRange(songs.Select(s => new Song
    {
        Id = s.Id,
        Name = s.Name,
        MimeType = s.MimeType,
        Guid = s.Guid,
    }));
}

void ExtractSongRequests()
{
    var songRequests = mssql.SongRequests.ToList();
    psql.SongRequests.AddRange(songRequests.Select(sr => new SongRequest
    {
        Id = sr.Id,
        Name = sr.Name,
        MimeType = sr.MimeType,
        Source = sr.Source,
        RequestStatus = sr.RequestStatus,
        SourceUrl = sr.SourceUrl,
        UploaderAccountId = sr.UploaderAccountId,
    }));
}
