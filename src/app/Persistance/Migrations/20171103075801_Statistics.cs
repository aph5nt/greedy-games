using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Persistance.Migrations
{
    public partial class Statistics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "stats");

            migrationBuilder.CreateTable(
                name: "MinefieldStats",
                schema: "stats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Bet = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GameId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Loss = table.Column<long>(type: "bigint", nullable: false),
                    Network = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Turn = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Win = table.Column<long>(type: "bigint", nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MinefieldStats",
                schema: "stats");
        }
    }
}
