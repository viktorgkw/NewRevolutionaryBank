namespace NewRevolutionaryBank.Data.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class BankSettings_TiersTaxes_Added : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MonthlyTax",
            table: "BankSettings");

        migrationBuilder.AddColumn<decimal>(
            name: "PremiumTax",
            table: "BankSettings",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            defaultValue: 0m,
            comment: "Месечна такса за Premium сметка");

        migrationBuilder.AddColumn<decimal>(
            name: "StandardTax",
            table: "BankSettings",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            defaultValue: 0m,
            comment: "Месечна такса за Standard сметка");

        migrationBuilder.AddColumn<decimal>(
            name: "VipTax",
            table: "BankSettings",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            defaultValue: 0m,
            comment: "Месечна такса за VIP сметка");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PremiumTax",
            table: "BankSettings");

        migrationBuilder.DropColumn(
            name: "StandardTax",
            table: "BankSettings");

        migrationBuilder.DropColumn(
            name: "VipTax",
            table: "BankSettings");

        migrationBuilder.AddColumn<decimal>(
            name: "MonthlyTax",
            table: "BankSettings",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            defaultValue: 0m,
            comment: "Месечна такса на банката");
    }
}
