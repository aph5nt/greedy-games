using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApi.Data.Migrations
{
    public partial class AuthChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TwoFactorAuthSecretHash",
                table: "AspNetUsers",
                newName: "TwoFactorAuthSecret");

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorAuthEnabled",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactorAuthEnabled",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "TwoFactorAuthSecret",
                table: "AspNetUsers",
                newName: "TwoFactorAuthSecretHash");
        }
    }
}
