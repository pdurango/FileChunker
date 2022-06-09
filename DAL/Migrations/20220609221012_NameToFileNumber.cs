using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class NameToFileNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ChunkInfo");

            migrationBuilder.AddColumn<int>(
                name: "FileNumber",
                table: "ChunkInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileNumber",
                table: "ChunkInfo");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ChunkInfo",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
