using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewRevolutionaryBank.Data.Migrations
{
    /// <inheritdoc />
    public partial class IBAN_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IBAN",
                table: "BankAccounts",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IBAN",
                table: "BankAccounts");
        }
    }
}
