using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Songs_Id",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_SongRequests_Id",
                table: "SongRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Songs_Id",
                table: "Songs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SongRequests_Id",
                table: "SongRequests",
                column: "Id");
        }
    }
}
