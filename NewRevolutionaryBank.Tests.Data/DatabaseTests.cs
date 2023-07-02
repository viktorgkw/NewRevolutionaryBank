namespace NewRevolutionaryBank.Tests.Data;

using System.Linq;

using Microsoft.EntityFrameworkCore;
using Xunit;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;

public class DatabaseTests
{
	private readonly DbContextOptions<NrbDbContext> dbContextOptions;

	public DatabaseTests() =>
		dbContextOptions = new DbContextOptionsBuilder<NrbDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

	[Fact]
	public void Can_Add_BankSettings_To_Database()
	{
		// Arrange
		decimal transactionFee = 0.20m;

		using (NrbDbContext dbContext = new(dbContextOptions))
		{
			BankSettings bankSettings = new()
			{
				Id = Guid.NewGuid(),
				TransactionFee = transactionFee
			};

			// Act
			dbContext.BankSettings.Add(bankSettings);
			dbContext.SaveChanges();
		}

		// Assert
		using (NrbDbContext dbContext = new(dbContextOptions))
		{
			Assert.Single(dbContext.BankSettings);
			Assert.Equal(transactionFee, dbContext.BankSettings.First().TransactionFee);
		}
	}

	[Fact]
	public void Can_Get_Transactions_From_Database()
	{
		// Arrange
		using (NrbDbContext dbContext = new(dbContextOptions))
		{
			ApplicationUser user = new()
			{
				UserName = "TestUser",
				Email = "testuseremail@example.com",
				EmailConfirmed = true
			};

			dbContext.Users.Add(user);

			BankAccount accountFrom = new()
			{
				IBAN = "Sample IBAN 1",
				Balance = 1000.00m,
				OwnerId = user.Id,
				Owner = user,
				UnifiedCivilNumber = "1234567890",
				Address = "Sample Address 1",
				IsClosed = false
			};

			BankAccount accountTo = new()
			{
				IBAN = "Sample IBAN 2",
				Balance = 2000.00m,
				OwnerId = user.Id,
				Owner = user,
				UnifiedCivilNumber = "0987654321",
				Address = "Sample Address 2",
				IsClosed = false
			};

			dbContext.BankAccounts.Add(accountFrom);
			dbContext.BankAccounts.Add(accountTo);
			dbContext.SaveChanges();

			Transaction transaction1 = new()
			{
				TransactionDate = DateTime.Now,
				Amount = 100.00m,
				Description = "Sample transaction 1",
				AccountFromId = accountFrom.Id,
				AccountFrom = accountFrom,
				AccountToId = accountTo.Id,
				AccountTo = accountTo
			};

			Transaction transaction2 = new()
			{
				TransactionDate = DateTime.Now,
				Amount = 200.00m,
				Description = "Sample transaction 2",
				AccountFromId = accountFrom.Id,
				AccountFrom = accountFrom,
				AccountToId = accountTo.Id,
				AccountTo = accountTo
			};

			dbContext.Transactions.Add(transaction1);
			dbContext.Transactions.Add(transaction2);
			dbContext.SaveChanges();
		}

		// Act
		using (NrbDbContext dbContext = new(dbContextOptions))
		{
			List<Transaction> transactions = dbContext.Transactions.ToList();

			// Assert
			Assert.Equal(2, transactions.Count);
		}
	}

}
