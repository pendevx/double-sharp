using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSongRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlSongRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UrlSongRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UploaderAccountId = table.Column<int>(type: "integer", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", unicode: false, maxLength: 256, nullable: false),
                    RequestStatus = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(2048)", unicode: false, maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SongRequests_Accounts_UploaderAccountId",
                        column: x => x.UploaderAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SongRequests_UploaderAccountId",
                table: "UrlSongRequests",
                column: "UploaderAccountId");
        }
    }
}
