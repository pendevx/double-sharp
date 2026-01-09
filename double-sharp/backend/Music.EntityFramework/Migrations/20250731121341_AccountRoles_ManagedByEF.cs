using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AccountRoles_ManagedByEF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountRoles_Accounts_AccountId",
                table: "AccountRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountRoles_Roles_RoleId",
                table: "AccountRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountRoles",
                table: "AccountRoles");

            migrationBuilder.RenameTable(
                name: "AccountRoles",
                newName: "AccountRole");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "AccountRole",
                newName: "RolesId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "AccountRole",
                newName: "AccountsId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountRoles_RoleId",
                table: "AccountRole",
                newName: "IX_AccountRole_RolesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountRole",
                table: "AccountRole",
                columns: new[] { "AccountsId", "RolesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRole_Accounts_AccountsId",
                table: "AccountRole",
                column: "AccountsId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRole_Roles_RolesId",
                table: "AccountRole",
                column: "RolesId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountRole_Accounts_AccountsId",
                table: "AccountRole");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountRole_Roles_RolesId",
                table: "AccountRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountRole",
                table: "AccountRole");

            migrationBuilder.RenameTable(
                name: "AccountRole",
                newName: "AccountRoles");

            migrationBuilder.RenameColumn(
                name: "RolesId",
                table: "AccountRoles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "AccountsId",
                table: "AccountRoles",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountRole_RolesId",
                table: "AccountRoles",
                newName: "IX_AccountRoles_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountRoles",
                table: "AccountRoles",
                columns: new[] { "AccountId", "RoleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRoles_Accounts_AccountId",
                table: "AccountRoles",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRoles_Roles_RoleId",
                table: "AccountRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
