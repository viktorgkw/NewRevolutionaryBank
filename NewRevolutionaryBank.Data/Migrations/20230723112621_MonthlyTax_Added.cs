namespace NewRevolutionaryBank.Data.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class MonthlyTax_Added : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder) =>
		migrationBuilder.AddColumn<decimal>(
			name: "MonthlyTax",
			table: "BankSettings",
			type: "decimal(18,2)",
			precision: 18,
			scale: 2,
			nullable: false,
			defaultValue: 0m,
			comment: "Месечна такса на банката");

	protected override void Down(MigrationBuilder migrationBuilder) =>
		migrationBuilder.DropColumn(
			name: "MonthlyTax",
			table: "BankSettings");
}
