using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApi.Data.Migrations
{
    public partial class TwoFactorAuth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatToken",
                table: "AspNetUsers",
                newName: "TwoFactorAuthSecretHash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TwoFactorAuthSecretHash",
                table: "AspNetUsers",
                newName: "ChatToken");
        }
    }
}
