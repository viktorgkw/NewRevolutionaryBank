namespace NewRevolutionaryBank.Tests.Services;

using Microsoft.EntityFrameworkCore;
using Xunit;

using NewRevolutionaryBank.Data.Models.Enums;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Web.ViewModels.Transaction;
using NewRevolutionaryBank.Services;
using NewRevolutionaryBank.Tests.Services.Mocks;
using NewRevolutionaryBank.Services.Messaging.Contracts;

public class TransactionServiceTests
{
	private readonly NrbDbContext _dbContext;
	private readonly IEmailSender _emailSender;
	private readonly TransactionService _transactionService;

	public TransactionServiceTests()
	{
		DbContextOptions<NrbDbContext> options = new DbContextOptionsBuilder<NrbDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_dbContext = new NrbDbContext(options);
		_emailSender = new MockEmailSender();
		_transactionService = new TransactionService(_dbContext, _emailSender);
	}

	[Fact]
	public async Task PrepareTransactionModelForUserAsync_ReturnsNonNullModel()
	{
		// Arrange
		string userName = "testUser";
		Guid bankAccId = Guid.NewGuid();

		ApplicationUser user = new()
		{
			UserName = userName,
			BankAccounts = new List<BankAccount>
			{
				new()
				{
					Id = bankAccId,
					IBAN = "123456789",
					Balance = 100,
					Address = "Test address from",
					UnifiedCivilNumber = "0123456789"
				}
			}
		};

		_dbContext.Users.Add(user);
		_dbContext.SaveChanges();

		// Act
		TransactionNewViewModel result = await
			_transactionService.PrepareTransactionModelForUserAsync(userName);

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.SenderAccounts);
		Assert.Single(result.SenderAccounts);
		Assert.Equal(bankAccId, result.SenderAccounts[0].Id);
		Assert.Equal("123456789", result.SenderAccounts[0].IBAN);
		Assert.Equal(100, result.SenderAccounts[0].Balance);
	}

	[Fact]
	public async Task BeginPaymentAsync_WithValidModel_SuccessfulPayment()
	{
		// Arrange
		string accToIBAN = "1716151413121110987654321";

		BankAccount accountFrom = new()
		{
			Id = Guid.NewGuid(),
			Balance = 100,
			Address = "Test address from",
			IBAN = "1234567891011121314151617",
			UnifiedCivilNumber = "0123456789"
		};
		BankAccount accountTo = new()
		{
			Id = Guid.NewGuid(),
			Balance = 0,
			Address = "Test address to",
			IBAN = accToIBAN,
			UnifiedCivilNumber = "0123456789"
		};

		ApplicationUser userFrom = new() { Email = "from@test.com" };
		ApplicationUser userTo = new() { Email = "to@test.com" };
		BankSettings bankSettings = new() { TransactionFee = 1 };

		userFrom.BankAccounts.Add(accountFrom);
		userTo.BankAccounts.Add(accountTo);

		TransactionNewViewModel model = new()
		{
			AccountFrom = accountFrom.Id.ToString(),
			AccountTo = accToIBAN,
			Amount = 50,
			Description = "Test payment"
		};

		_dbContext.BankSettings.Add(bankSettings);
		_dbContext.Users.Add(userFrom);
		_dbContext.Users.Add(userTo);
		_dbContext.BankAccounts.Add(accountFrom);
		_dbContext.BankAccounts.Add(accountTo);
		_dbContext.SaveChanges();

		decimal fromAmount = 49;
		decimal toAmount = 50;

		// Act
		PaymentResult result = await _transactionService.BeginPaymentAsync(model);

		// Assert
		Assert.Equal(PaymentResult.Successful, result);

		// Verify that the balances were updated successfully.
		Assert.Equal(fromAmount, accountFrom.Balance);
		Assert.Equal(toAmount, accountTo.Balance);

		// Verify that the transaction was added to the db.
		List<Transaction> transactions = _dbContext.Transactions.ToList();
		Assert.Single(transactions);
		Transaction transaction = transactions[0];
		Assert.Equal("Test payment", transaction.Description);
		Assert.Equal(50, transaction.Amount);
	}

	[Fact]
	public async Task BeginPaymentAsync_WithInsufficientFunds_ReturnsInsufficientFunds()
	{
		// Arrange
		string accFromIBAN = "1234567891011121314151617";
		string accToIBAN = "1716151413121110987654321";

		ApplicationUser userFrom = new() { Email = "from@test.com" };
		ApplicationUser userTo = new() { Email = "to@test.com" };

		BankAccount accountFrom = new()
		{
			Id = Guid.NewGuid(),
			Balance = 100,
			Address = "Test address from",
			IBAN = accFromIBAN,
			UnifiedCivilNumber = "0123456789"
		};

		BankAccount accountTo = new()
		{
			Id = Guid.NewGuid(),
			Balance = 0,
			IBAN = accToIBAN,
			Address = "Test address from",
			UnifiedCivilNumber = "0123456789"
		};

		userFrom.BankAccounts.Add(accountFrom);
		userTo.BankAccounts.Add(accountTo);

		_dbContext.BankSettings.Add(new() { TransactionFee = 1 });
		_dbContext.Users.Add(userFrom);
		_dbContext.Users.Add(userTo);
		_dbContext.BankAccounts.Add(accountFrom);
		_dbContext.BankAccounts.Add(accountTo);
		_dbContext.SaveChanges();

		TransactionNewViewModel model = new()
		{
			AccountFrom = accountFrom.Id.ToString(),
			AccountTo = accountTo.IBAN,
			Amount = 150,
			Description = "Test payment"
		};

		// Act
		PaymentResult result = await _transactionService.BeginPaymentAsync(model);

		// Assert
		Assert.Equal(PaymentResult.InsufficientFunds, result);
		Assert.Equal(100, accountFrom.Balance);
		Assert.Equal(0, accountTo.Balance);
		Assert.Empty(_dbContext.Transactions);
	}

	[Fact]
	public async Task BeginPaymentAsync_WithSenderNotFound_ReturnsSenderNotFound()
	{
		// Arrange
		string accToIBAN = "1716151413121110987654321";

		BankAccount accountTo = new()
		{
			Id = Guid.NewGuid(),
			Balance = 0,
			IBAN = accToIBAN,
			Address = "Test address from",
			UnifiedCivilNumber = "0123456789"
		};

		_dbContext.BankAccounts.Add(accountTo);
		_dbContext.SaveChanges();

		TransactionNewViewModel model = new()
		{
			AccountFrom = "None",
			AccountTo = accountTo.IBAN,
			Amount = 50,
			Description = "Test payment"
		};

		// Act
		PaymentResult result = await _transactionService.BeginPaymentAsync(model);

		// Assert
		Assert.Equal(PaymentResult.SenderNotFound, result);
		Assert.Equal(0, accountTo.Balance);
		Assert.Empty(_dbContext.Transactions);
	}

	[Fact]
	public async Task BeginPaymentAsync_WithReceiverNotFound_ReturnsReceiverNotFound()
	{
		// Arrange
		string accFromIBAN = "1716151413121110987654321";

		BankAccount accountFrom = new()
		{
			Id = Guid.NewGuid(),
			Balance = 100,
			Address = "Test address from",
			IBAN = accFromIBAN,
			UnifiedCivilNumber = "0123456789"
		};

		TransactionNewViewModel model = new()
		{
			AccountFrom = accountFrom.Id.ToString(),
			AccountTo = "None",
			Amount = 50,
			Description = "Test payment"
		};

		_dbContext.BankAccounts.Add(accountFrom);
		_dbContext.SaveChanges();

		// Act
		PaymentResult result = await _transactionService.BeginPaymentAsync(model);

		// Assert
		Assert.Equal(PaymentResult.RecieverNotFound, result);
		Assert.Equal(100, accountFrom.Balance);
		Assert.Empty(_dbContext.Transactions);
	}

	[Fact]
	public async Task BeginPaymentAsync_WithSelfTransaction_ReturnsNoSelfTransactions()
	{
		// Arrange
		string accIBAN = "1716151413121110987654321";

		BankAccount accountFrom = new()
		{
			Id = Guid.NewGuid(),
			Balance = 100,
			IBAN = accIBAN,
			Address = "Test address from",
			UnifiedCivilNumber = "0123456789"
		};

		TransactionNewViewModel model = new TransactionNewViewModel
		{
			AccountFrom = accountFrom.Id.ToString(),
			AccountTo = accountFrom.IBAN,
			Amount = 50,
			Description = "Test payment"
		};

		_dbContext.BankAccounts.Add(accountFrom);
		_dbContext.SaveChanges();

		// Act
		PaymentResult result = await _transactionService.BeginPaymentAsync(model);

		// Assert
		Assert.Equal(PaymentResult.NoSelfTransactions, result);
		Assert.Equal(100, accountFrom.Balance);
		Assert.Empty(_dbContext.Transactions);
	}
}
