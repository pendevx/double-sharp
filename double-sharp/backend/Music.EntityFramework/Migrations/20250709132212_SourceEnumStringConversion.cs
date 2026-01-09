using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SourceEnumStringConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequestSource_Source",
                table: "SongRequests",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RequestSource_Source",
                table: "SongRequests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
