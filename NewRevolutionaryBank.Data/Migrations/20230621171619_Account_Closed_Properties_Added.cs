using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRevolutionaryBank.Data.Migrations
{
    /// <inheritdoc />
    public partial class Account_Closed_Properties_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedDate",
                table: "BankAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "BankAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedDate",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "BankAccounts");
        }
    }
}
