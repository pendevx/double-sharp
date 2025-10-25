using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class Artists_Slug_Unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "_slug_Value",
                table: "Artists",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "_requestInformation_Status",
                table: "Artists",
                newName: "RequestStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_Slug",
                table: "Artists",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Artists_Slug",
                table: "Artists");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "Artists",
                newName: "_slug_Value");

            migrationBuilder.RenameColumn(
                name: "RequestStatus",
                table: "Artists",
                newName: "_requestInformation_Status");
        }
    }
}
