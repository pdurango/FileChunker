using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChunkInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaInfoId = table.Column<int>(type: "int", nullable: true),
                    LocationInfoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChunkInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChunkInfo_LocationInfo_LocationInfoId",
                        column: x => x.LocationInfoId,
                        principalTable: "LocationInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChunkInfo_MetaInfo_MetaInfoId",
                        column: x => x.MetaInfoId,
                        principalTable: "MetaInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChunkInfo_LocationInfoId",
                table: "ChunkInfo",
                column: "LocationInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChunkInfo_MetaInfoId",
                table: "ChunkInfo",
                column: "MetaInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChunkInfo");

            migrationBuilder.DropTable(
                name: "LocationInfo");

            migrationBuilder.DropTable(
                name: "MetaInfo");
        }
    }
}
