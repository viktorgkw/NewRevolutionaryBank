namespace NewRevolutionaryBank.Data.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class Deposit_Table_Added : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Deposits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AccountToId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                CVC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ExpYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ExpMonth = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Deposits", x => x.Id);
                table.ForeignKey(
                    name: "FK_Deposits_BankAccounts_AccountToId",
                    column: x => x.AccountToId,
                    principalTable: "BankAccounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Deposits_AccountToId",
            table: "Deposits",
            column: "AccountToId");
    }

	protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(
			name: "Deposits");
}
