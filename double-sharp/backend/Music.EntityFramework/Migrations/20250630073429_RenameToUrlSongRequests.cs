using Microsoft.EntityFrameworkCore.Migrations;
using Music.Models.Data.SongRequests;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RenameToUrlSongRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "SongRequests",
                newName: "UrlSongRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UrlSongRequests",
                newName: "SongRequests");
        }
    }
}
