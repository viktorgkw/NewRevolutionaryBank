namespace NewRevolutionaryBank.Data.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class BankSettings_BankBalance_Prop_Added : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "BankBalance",
            table: "BankSettings",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            defaultValue: 0m,
            comment: "Баланс на банката");

        migrationBuilder.AlterColumn<string>(
            name: "LastName",
            table: "AspNetUsers",
            type: "nvarchar(46)",
            maxLength: 46,
            nullable: false,
            defaultValue: "",
            comment: "Фамилия",
            oldClrType: typeof(string),
            oldType: "nvarchar(46)",
            oldMaxLength: 46,
            oldNullable: true,
            oldComment: "Фамилия");

        migrationBuilder.AlterColumn<string>(
            name: "FirstName",
            table: "AspNetUsers",
            type: "nvarchar(46)",
            maxLength: 46,
            nullable: false,
            defaultValue: "",
            comment: "Собствено име",
            oldClrType: typeof(string),
            oldType: "nvarchar(46)",
            oldMaxLength: 46,
            oldNullable: true,
            oldComment: "Собствено име");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BankBalance",
            table: "BankSettings");

        migrationBuilder.AlterColumn<string>(
            name: "LastName",
            table: "AspNetUsers",
            type: "nvarchar(46)",
            maxLength: 46,
            nullable: true,
            comment: "Фамилия",
            oldClrType: typeof(string),
            oldType: "nvarchar(46)",
            oldMaxLength: 46,
            oldComment: "Фамилия");

        migrationBuilder.AlterColumn<string>(
            name: "FirstName",
            table: "AspNetUsers",
            type: "nvarchar(46)",
            maxLength: 46,
            nullable: true,
            comment: "Собствено име",
            oldClrType: typeof(string),
            oldType: "nvarchar(46)",
            oldMaxLength: 46,
            oldComment: "Собствено име");
    }
}
