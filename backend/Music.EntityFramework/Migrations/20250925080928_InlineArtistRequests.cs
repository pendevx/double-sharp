using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class InlineArtistRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistRequests");

            migrationBuilder.AddColumn<int>(
                name: "_requestInformation_RequesterId",
                table: "Artists",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "_requestInformation_Status",
                table: "Artists",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artists__requestInformation_RequesterId",
                table: "Artists",
                column: "_requestInformation_RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Accounts__requestInformation_RequesterId",
                table: "Artists",
                column: "_requestInformation_RequesterId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Accounts__requestInformation_RequesterId",
                table: "Artists");

            migrationBuilder.DropIndex(
                name: "IX_Artists__requestInformation_RequesterId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "_requestInformation_RequesterId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "_requestInformation_Status",
                table: "Artists");

            migrationBuilder.CreateTable(
                name: "ArtistRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArtistId = table.Column<int>(type: "integer", nullable: true),
                    RequesterId = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistRequests_Accounts_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistRequests_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistRequests_ArtistId",
                table: "ArtistRequests",
                column: "ArtistId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArtistRequests_RequesterId",
                table: "ArtistRequests",
                column: "RequesterId");
        }
    }
}
