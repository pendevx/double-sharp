using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.Models.Data.Migrations
{
    /// <inheritdoc />
    public partial class SongClass_AddContentsOid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contents",
                table: "Songs");

            migrationBuilder.AddColumn<int>(
                name: "ContentsOid",
                table: "Songs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentsOid",
                table: "Songs");

            migrationBuilder.AddColumn<byte[]>(
                name: "Contents",
                table: "Songs",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
