namespace NewRevolutionaryBank.Data.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class Db_Comments_Added : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterTable(
            name: "Transactions",
            comment: "Банкова транзакция");

        migrationBuilder.AlterTable(
            name: "Ratings",
            comment: "Рейтинг на уебсайта");

        migrationBuilder.AlterTable(
            name: "Deposits",
            comment: "Банков депозит");

        migrationBuilder.AlterTable(
            name: "BankSettings",
            comment: "Банкови настройки");

        migrationBuilder.AlterTable(
            name: "BankAccounts",
            comment: "Банкова сметка",
            oldComment: "Банква сметка");

        migrationBuilder.AlterColumn<int>(
            name: "RateValue",
            table: "Ratings",
            type: "int",
            nullable: false,
            comment: "Стойност на рейтинга",
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "Ratings",
            type: "uniqueidentifier",
            nullable: false,
            comment: "Уникален идентификатор",
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");

        migrationBuilder.AlterColumn<string>(
            name: "ExpYear",
            table: "Deposits",
            type: "nvarchar(max)",
            nullable: false,
            comment: "Година на изтичане на карата с която се прави депозит",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "ExpMonth",
            table: "Deposits",
            type: "nvarchar(max)",
            nullable: false,
            comment: "Месец на изтичане на карата с която се прави депозит",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<DateTime>(
            name: "DepositedAt",
            table: "Deposits",
            type: "datetime2",
            nullable: false,
            comment: "Дата на направения депозит",
            oldClrType: typeof(DateTime),
            oldType: "datetime2");

        migrationBuilder.AlterColumn<string>(
            name: "CardNumber",
            table: "Deposits",
            type: "nvarchar(max)",
            nullable: false,
            comment: "Номер на картата с която се прави депозит",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "CVC",
            table: "Deposits",
            type: "nvarchar(max)",
            nullable: false,
            comment: "CVC на картата с която се прави депозит",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<decimal>(
            name: "Amount",
            table: "Deposits",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            comment: "Стойност на депозит",
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldPrecision: 18,
            oldScale: 2);

        migrationBuilder.AlterColumn<Guid>(
            name: "AccountToId",
            table: "Deposits",
            type: "uniqueidentifier",
            nullable: false,
            comment: "Уникален идентификатор на получател",
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "Deposits",
            type: "uniqueidentifier",
            nullable: false,
            comment: "Уникален идентификатор",
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterTable(
            name: "Transactions",
            oldComment: "Банкова транзакция");

        migrationBuilder.AlterTable(
            name: "Ratings",
            oldComment: "Рейтинг на уебсайта");

        migrationBuilder.AlterTable(
            name: "Deposits",
            oldComment: "Банков депозит");

        migrationBuilder.AlterTable(
            name: "BankSettings",
            oldComment: "Банкови настройки");

        migrationBuilder.AlterTable(
            name: "BankAccounts",
            comment: "Банква сметка",
            oldComment: "Банкова сметка");

        migrationBuilder.AlterColumn<int>(
            name: "RateValue",
            table: "Ratings",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int",
            oldComment: "Стойност на рейтинга");

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "Ratings",
            type: "uniqueidentifier",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldComment: "Уникален идентификатор");

        migrationBuilder.AlterColumn<string>(
            name: "ExpYear",
            table: "Deposits",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldComment: "Година на изтичане на карата с която се прави депозит");

        migrationBuilder.AlterColumn<string>(
            name: "ExpMonth",
            table: "Deposits",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldComment: "Месец на изтичане на карата с която се прави депозит");

        migrationBuilder.AlterColumn<DateTime>(
            name: "DepositedAt",
            table: "Deposits",
            type: "datetime2",
            nullable: false,
            oldClrType: typeof(DateTime),
            oldType: "datetime2",
            oldComment: "Дата на направения депозит");

        migrationBuilder.AlterColumn<string>(
            name: "CardNumber",
            table: "Deposits",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldComment: "Номер на картата с която се прави депозит");

        migrationBuilder.AlterColumn<string>(
            name: "CVC",
            table: "Deposits",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldComment: "CVC на картата с която се прави депозит");

        migrationBuilder.AlterColumn<decimal>(
            name: "Amount",
            table: "Deposits",
            type: "decimal(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldPrecision: 18,
            oldScale: 2,
            oldComment: "Стойност на депозит");

        migrationBuilder.AlterColumn<Guid>(
            name: "AccountToId",
            table: "Deposits",
            type: "uniqueidentifier",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldComment: "Уникален идентификатор на получател");

        migrationBuilder.AlterColumn<Guid>(
            name: "Id",
            table: "Deposits",
            type: "uniqueidentifier",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldComment: "Уникален идентификатор");
    }
}
