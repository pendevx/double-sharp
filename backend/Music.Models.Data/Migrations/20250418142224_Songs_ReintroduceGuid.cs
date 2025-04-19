using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.Models.Data.Migrations
{
    /// <inheritdoc />
    public partial class Songs_ReintroduceGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ__Sessions__1EB4F817BC4B8EF7",
                table: "Sessions");

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Songs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Songs");

            migrationBuilder.CreateIndex(
                name: "UQ__Sessions__1EB4F817BC4B8EF7",
                table: "Sessions",
                column: "Token",
                unique: true);
        }
    }
}
