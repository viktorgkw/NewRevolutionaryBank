namespace NewRevolutionaryBank.Data.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class Deposit_Date_Create_Add : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<DateTime>(
			name: "DepositedAt",
			table: "Deposits",
			type: "datetime2",
			nullable: false,
			defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

	protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(
			name: "DepositedAt",
			table: "Deposits");
}
