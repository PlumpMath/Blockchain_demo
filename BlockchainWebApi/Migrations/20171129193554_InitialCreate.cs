using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Blockchain_example.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    Index = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<byte[]>(nullable: true),
                    Hash = table.Column<string>(nullable: true),
                    PrevHash = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Index);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blocks");
        }
    }
}
