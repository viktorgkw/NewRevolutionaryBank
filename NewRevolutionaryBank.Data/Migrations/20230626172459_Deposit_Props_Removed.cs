namespace NewRevolutionaryBank.Data.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class Deposit_Props_Removed : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CustomerName",
            table: "Deposits");

        migrationBuilder.DropColumn(
            name: "Description",
            table: "Deposits");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CustomerName",
            table: "Deposits",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "Deposits",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
    }
}
