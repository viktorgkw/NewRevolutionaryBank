namespace NewRevolutionaryBank.Data.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class Restrictions_Added : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "LastName",
            table: "AspNetUsers",
            type: "nvarchar(46)",
            maxLength: 46,
            nullable: true,
            comment: "Фамилия",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true,
            oldComment: "Фамилия");

        migrationBuilder.AlterColumn<string>(
            name: "FirstName",
            table: "AspNetUsers",
            type: "nvarchar(46)",
            maxLength: 46,
            nullable: true,
            comment: "Собствено име",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true,
            oldComment: "Собствено име");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "LastName",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true,
            comment: "Фамилия",
            oldClrType: typeof(string),
            oldType: "nvarchar(46)",
            oldMaxLength: 46,
            oldNullable: true,
            oldComment: "Фамилия");

        migrationBuilder.AlterColumn<string>(
            name: "FirstName",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true,
            comment: "Собствено име",
            oldClrType: typeof(string),
            oldType: "nvarchar(46)",
            oldMaxLength: 46,
            oldNullable: true,
            oldComment: "Собствено име");
    }
}
