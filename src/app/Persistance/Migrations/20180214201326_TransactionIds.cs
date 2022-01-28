using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Persistance.Migrations
{
    public partial class TransactionIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionHeight",
                table: "Withdraws");

            migrationBuilder.DropColumn(
                name: "TransactionHeight",
                table: "Deposits");

            migrationBuilder.DropColumn(
                name: "TransactionSignature",
                table: "Deposits");

            migrationBuilder.RenameColumn(
                name: "TransactionSignature",
                table: "Withdraws",
                newName: "TransactionId");

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Deposits",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Deposits");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Withdraws",
                newName: "TransactionSignature");

            migrationBuilder.AddColumn<long>(
                name: "TransactionHeight",
                table: "Withdraws",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TransactionHeight",
                table: "Deposits",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "TransactionSignature",
                table: "Deposits",
                nullable: false,
                defaultValue: "");
        }
    }
}
