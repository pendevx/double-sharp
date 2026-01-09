using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RolePermissions_ManagedByEF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "PermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "PermissionRole");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "PermissionRole",
                newName: "IX_PermissionRole_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Permissions_PermissionId",
                table: "PermissionRole",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Roles_RoleId",
                table: "PermissionRole",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Permissions_PermissionId",
                table: "PermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Roles_RoleId",
                table: "PermissionRole");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionRole_RoleId",
                table: "PermissionRole",
                newName: "IX_RolePermissions_PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Permissions_PermissionId",
                table: "PermissionRole",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "PermissionRole",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
