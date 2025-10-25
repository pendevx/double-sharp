using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class Artists_RequesterIdRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Accounts__requestInformation_RequesterId",
                table: "Artists");

            migrationBuilder.RenameColumn(
                name: "_requestInformation_RequesterId",
                table: "Artists",
                newName: "RequesterId");

            migrationBuilder.RenameIndex(
                name: "IX_Artists__requestInformation_RequesterId",
                table: "Artists",
                newName: "IX_Artists_RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Accounts_RequesterId",
                table: "Artists",
                column: "RequesterId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Accounts_RequesterId",
                table: "Artists");

            migrationBuilder.RenameColumn(
                name: "RequesterId",
                table: "Artists",
                newName: "_requestInformation_RequesterId");

            migrationBuilder.RenameIndex(
                name: "IX_Artists_RequesterId",
                table: "Artists",
                newName: "IX_Artists__requestInformation_RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Accounts__requestInformation_RequesterId",
                table: "Artists",
                column: "_requestInformation_RequesterId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
