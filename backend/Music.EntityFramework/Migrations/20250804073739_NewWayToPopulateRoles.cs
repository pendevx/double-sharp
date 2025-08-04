using Microsoft.EntityFrameworkCore.Migrations;
using Music.Models.Data;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class NewWayToPopulateRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData( // from 20250621134933_PopulateRoles.cs
                table: "Roles",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2 });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "User" },
                    { 2, "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.InsertData( // from 20250621134933_PopulateRoles.cs
                table: "Roles",
                columns: ["Id", "Name"],
                values: new object[,]
                {
                    { 1, "User" },
                    { 2, "Admin" },
                });
        }
    }
}
