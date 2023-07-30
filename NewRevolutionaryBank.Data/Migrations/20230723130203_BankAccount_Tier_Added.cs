namespace NewRevolutionaryBank.Data.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class BankAccount_Tier_Added : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder) =>
		migrationBuilder.AddColumn<int>(
			name: "Tier",
			table: "BankAccounts",
			type: "int",
			nullable: false,
			defaultValue: 0,
			comment: "Ниво на сметката");

	protected override void Down(MigrationBuilder migrationBuilder) =>
		migrationBuilder.DropColumn(
			name: "Tier",
			table: "BankAccounts");
}
