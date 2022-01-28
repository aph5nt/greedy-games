using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Persistance.Migrations
{
    public partial class Rename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinefieldStats",
                schema: "stats");

            migrationBuilder.CreateTable(
                name: "GameStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Bet = table.Column<long>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    GameId = table.Column<string>(maxLength: 255, nullable: false),
                    Loss = table.Column<long>(nullable: false),
                    Network = table.Column<int>(nullable: false),
                    Size = table.Column<string>(maxLength: 6, nullable: false),
                    Turn = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Win = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStatistics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_AllUsersQuery",
                table: "GameStatistics",
                columns: new[] { "Type", "Network", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IDX_UserQuery",
                table: "GameStatistics",
                columns: new[] { "Type", "Network", "UserName", "CreatedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameStatistics");

            migrationBuilder.EnsureSchema(
                name: "stats");

            migrationBuilder.CreateTable(
                name: "MinefieldStats",
                schema: "stats",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Bet = table.Column<long>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    GameId = table.Column<string>(maxLength: 255, nullable: false),
                    Loss = table.Column<long>(nullable: false),
                    Network = table.Column<int>(nullable: false),
                    Size = table.Column<string>(maxLength: 6, nullable: false),
                    Turn = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Win = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinefieldStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_AllUsersQuery",
                schema: "stats",
                table: "MinefieldStats",
                columns: new[] { "Type", "Network", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IDX_UserQuery",
                schema: "stats",
                table: "MinefieldStats",
                columns: new[] { "Type", "Network", "UserName", "CreatedAt" });
        }
    }
}
