using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RemoveContentsOidFromSongs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentsOid",
                table: "Songs");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_Id",
                table: "Songs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Songs_Id",
                table: "Songs");

            migrationBuilder.AddColumn<uint>(
                name: "ContentsOid",
                table: "Songs",
                type: "oid",
                nullable: false,
                defaultValue: 0u);
        }
    }
}
