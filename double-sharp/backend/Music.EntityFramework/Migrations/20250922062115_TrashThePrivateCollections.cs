using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class TrashThePrivateCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistSong_Artists__artistsId",
                table: "ArtistSong");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistSong_Songs__songsId",
                table: "ArtistSong");

            migrationBuilder.RenameColumn(
                name: "_songsId",
                table: "ArtistSong",
                newName: "SongsId");

            migrationBuilder.RenameColumn(
                name: "_artistsId",
                table: "ArtistSong",
                newName: "ArtistsId");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistSong__songsId",
                table: "ArtistSong",
                newName: "IX_ArtistSong_SongsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistSong_Artists_ArtistsId",
                table: "ArtistSong",
                column: "ArtistsId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistSong_Songs_SongsId",
                table: "ArtistSong",
                column: "SongsId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistSong_Artists_ArtistsId",
                table: "ArtistSong");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistSong_Songs_SongsId",
                table: "ArtistSong");

            migrationBuilder.RenameColumn(
                name: "SongsId",
                table: "ArtistSong",
                newName: "_songsId");

            migrationBuilder.RenameColumn(
                name: "ArtistsId",
                table: "ArtistSong",
                newName: "_artistsId");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistSong_SongsId",
                table: "ArtistSong",
                newName: "IX_ArtistSong__songsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistSong_Artists__artistsId",
                table: "ArtistSong",
                column: "_artistsId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistSong_Songs__songsId",
                table: "ArtistSong",
                column: "_songsId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
