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
            table: "Roles",
            columns: ["Id", "Name"],
            values: new object[,]
            {
                { 1, "User" },
                { 2, "Admin" },
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Roles",
            keyColumn: "Id",
            keyValues: new object[] { 1, 2 });
    }
}
