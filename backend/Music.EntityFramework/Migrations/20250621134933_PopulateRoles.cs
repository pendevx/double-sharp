using Microsoft.EntityFrameworkCore.Migrations;
using Music.EntityFramework;
using Music.Models.Data;

#nullable disable

namespace Music.EntityFramework.Migrations;

/// <inheritdoc />
public partial class PopulateRoles : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: nameof(MusicContext.Roles),
            columns: [nameof(Role.Id), nameof(Role.Name)],
            values: new object[,]
            {
                { 1, nameof(RoleName.User) },
                { 2, nameof(RoleName.Admin) },
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: nameof(MusicContext.Roles),
            keyColumn: nameof(Role.Id),
            keyValues: new object[] { 1, 2 });
    }
}
