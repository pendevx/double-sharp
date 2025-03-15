using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.Models.Data.Migrations
{
    /// <inheritdoc />
    public partial class Songs_AddLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Length",
                table: "Songs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                table: "Songs");
        }
    }
}
