namespace NewRevolutionaryBank.Data.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class User_Navigation_Prop_Added : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropForeignKey(
			name: "FK_BankAccounts_AspNetUsers_ApplicationUserId",
			table: "BankAccounts");

		migrationBuilder.DropIndex(
			name: "IX_BankAccounts_ApplicationUserId",
			table: "BankAccounts");

		migrationBuilder.DropColumn(
			name: "ApplicationUserId",
			table: "BankAccounts");

		migrationBuilder.AddColumn<Guid>(
			name: "OwnerId",
			table: "BankAccounts",
			type: "uniqueidentifier",
			nullable: false,
			defaultValue: Guid.Empty,
			comment: "Уникален идентификатор на собственика на сметката");

		migrationBuilder.CreateIndex(
			name: "IX_BankAccounts_OwnerId",
			table: "BankAccounts",
			column: "OwnerId");

		migrationBuilder.AddForeignKey(
			name: "FK_BankAccounts_AspNetUsers_OwnerId",
			table: "BankAccounts",
			column: "OwnerId",
			principalTable: "AspNetUsers",
			principalColumn: "Id",
			onDelete: ReferentialAction.Cascade);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropForeignKey(
			name: "FK_BankAccounts_AspNetUsers_OwnerId",
			table: "BankAccounts");

		migrationBuilder.DropIndex(
			name: "IX_BankAccounts_OwnerId",
			table: "BankAccounts");

		migrationBuilder.DropColumn(
			name: "OwnerId",
			table: "BankAccounts");

		migrationBuilder.AddColumn<Guid>(
			name: "ApplicationUserId",
			table: "BankAccounts",
			type: "uniqueidentifier",
			nullable: true);

		migrationBuilder.CreateIndex(
			name: "IX_BankAccounts_ApplicationUserId",
			table: "BankAccounts",
			column: "ApplicationUserId");

		migrationBuilder.AddForeignKey(
			name: "FK_BankAccounts_AspNetUsers_ApplicationUserId",
			table: "BankAccounts",
			column: "ApplicationUserId",
			principalTable: "AspNetUsers",
			principalColumn: "Id");
	}
}
