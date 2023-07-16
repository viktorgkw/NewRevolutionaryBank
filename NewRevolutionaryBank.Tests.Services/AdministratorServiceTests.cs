namespace NewRevolutionaryBank.Tests.Services;

using Microsoft.EntityFrameworkCore;
using Xunit;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Services;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Web.ViewModels.Administrator;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;

public class AdministratorServiceTests
{
	private readonly DbContextOptions<NrbDbContext> _dbContextOptions;
	private readonly NrbDbContext _dbContext;
	private readonly IAdministratorService _administratorService;

	public AdministratorServiceTests()
	{
		_dbContextOptions = new DbContextOptionsBuilder<NrbDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_dbContext = new NrbDbContext(_dbContextOptions);

		InitializeData();

		_administratorService = new AdministratorService(_dbContext);
	}

	[Fact]
	public async Task ActivateBankAccountByIdAsync_ActivatesAccount()
	{
		// Arrange
		string accountId = _dbContext.BankAccounts.First().Id.ToString();

		// Act
		await _administratorService.ActivateBankAccountByIdAsync(accountId);

		// Assert
		BankAccount? bankAccount = _dbContext.BankAccounts
			.FirstOrDefault(ba => ba.Id.ToString() == accountId);

		Assert.NotNull(bankAccount);
		Assert.False(bankAccount.IsClosed);
		Assert.Null(bankAccount.ClosedDate);
	}

	[Fact]
	public async Task DeactivateBankAccountByIdAsync_DeactivatesAccount()
	{
		// Arrange
		BankAccount bankAcc = await _dbContext.BankAccounts.FirstAsync();

		// Act
		await _administratorService.DeactivateBankAccountByIdAsync(bankAcc.Id.ToString());

		// Assert
		BankAccount? bankAccount = await _dbContext.BankAccounts
			.FirstOrDefaultAsync(ba => ba.Id == bankAcc.Id);

		Assert.NotNull(bankAccount);
		Assert.True(bankAccount.IsClosed);
		Assert.NotNull(bankAccount.ClosedDate);
	}

	[Fact]
	public async Task GetAllBankAccountsAsync_Returns_AllBankAccounts()
	{
		// Act
		int expectedCount = 2;
		List<BankAccountManageViewModel> bankAccounts = await
			_administratorService.GetAllBankAccountsAsync();

		// Assert
		Assert.Equal(expectedCount, bankAccounts.Count);
	}

	[Fact]
	public async Task GetBankAccountDetailsAsync_Returns_BankAccountDetails()
	{
		// Arrange
		Guid accountId = _dbContext.BankAccounts.First().Id;

		// Act
		BankAccountDetailsViewModel accountDetails = await
			_administratorService.GetBankAccountDetailsByIdAsync(accountId);

		// Assert
		Assert.NotNull(accountDetails);
		Assert.Equal(accountId, accountDetails.Id);
		Assert.Equal("ABC123", accountDetails.IBAN);
		Assert.Equal("Some test address", accountDetails.Address);
	}

	[Fact]
	public async Task GetAllProfilesAsync_Returns_AllProfiles()
	{
		// Act
		int expectedCount = 2;
		List<UserProfileManageViewModel> userProfiles = await
			_administratorService.GetAllProfilesAsync(string.Empty, string.Empty);

		// Assert
		Assert.Equal(expectedCount, userProfiles.Count);
	}

	[Fact]
	public async Task GetAllProfilesAsync_Returns_AllProfilesWithOrder()
	{
		// Act
		string order = "deleted";
		int expectedCount = 0;

		List<UserProfileManageViewModel> userProfiles = await
			_administratorService.GetAllProfilesAsync(order, string.Empty);

		// Assert
		Assert.Equal(expectedCount, userProfiles.Count);
	}

	[Fact]
	public async Task GetAllProfilesAsync_Returns_AllProfilesWithSearchName()
	{
		// Act
		string order = "active";
		string searchName = "TeStUsErONE";
		int expectedCount = 1;

		List<UserProfileManageViewModel> userProfiles = await
			_administratorService.GetAllProfilesAsync(order, searchName);

		// Assert
		Assert.Equal(expectedCount, userProfiles.Count);
	}

	[Fact]
	public async Task GetUserProfileDetailsByIdAsync_Returns_UserProfileDetails()
	{
		// Arrange
		Guid userId = _dbContext.Users.First().Id;

		// Act
		UserProfileDetailsViewModel userDetails = await
			_administratorService.GetUserProfileDetailsByIdAsync(userId);

		// Assert
		Assert.NotNull(userDetails);
		Assert.Equal(userId, userDetails.Id);
		Assert.Equal("testuser1@example.com", userDetails.Email);
		Assert.Equal("TestUserOne", userDetails.UserName);
	}

	[Fact]
	public async Task ActivateUserProfileByIdAsync_ActivatesUserProfile()
	{
		// Arrange
		Guid userId = _dbContext.Users.First().Id;

		// Act
		await _administratorService.ActivateUserProfileByIdAsync(userId);

		// Assert
		ApplicationUser? user = _dbContext.Users
			.FirstOrDefault(u => u.Id == userId);

		Assert.NotNull(user);
		Assert.False(user.IsDeleted);
		Assert.Null(user.DeletedOn);
	}

	[Fact]
	public async Task DeactivateUserProfileByIdAsync_DeactivatesUserProfile()
	{
		// Arrange
		Guid userId = _dbContext.Users.First().Id;

		// Act
		await _administratorService.DeactivateUserProfileByIdAsync(userId);

		// Assert
		ApplicationUser? user = _dbContext.Users
			.FirstOrDefault(u => u.Id == userId);

		Assert.NotNull(user);
		Assert.True(user.IsDeleted);
		Assert.NotNull(user.DeletedOn);

		foreach (BankAccount bankAccount in user.BankAccounts)
		{
			Assert.True(bankAccount.IsClosed);
		}
	}

	[Fact]
	public async Task GetAllTransactionsAsync_ReturnsAllTransactions()
	{
		// Act
		int expectedCount = 1;
		List<TransactionDisplayViewModel> transactions = await _administratorService.GetAllTransactionsAsync();

		// Assert
		Assert.Equal(expectedCount, transactions.Count);
	}

	[Fact]
	public async Task GetBankSettingsAsync_Returns_BankSettings()
	{
		// Act
		BankSettingsDisplayViewModel bankSettings = await
			_administratorService.GetBankSettingsAsync();

		// Assert
		Assert.NotNull(bankSettings);
	}

	[Fact]
	public async Task EditTransactionFeeAsync_Updates_TransactionFee()
	{
		// Arrange
		decimal newTransactionFee = 0.20m;

		// Act
		await _administratorService.EditTransactionFeeAsync(newTransactionFee);

		// Assert
		BankSettings bankSettings = _dbContext.BankSettings.First();

		Assert.Equal(newTransactionFee, bankSettings.TransactionFee);
	}

	private void InitializeData()
	{
		ApplicationUser user1 = new()
		{
			Id = Guid.NewGuid(),
			UserName = "TestUserOne",
			FirstName = "FirstName",
			LastName = "LastName",
			Email = "testuser1@example.com",
			IsDeleted = false
		};

		ApplicationUser user2 = new()
		{
			Id = Guid.NewGuid(),
			UserName = "TestUserTwo",
			FirstName = "FirstName",
			LastName = "LastName",
			Email = "testuser2@example.com",
			IsDeleted = false
		};

		BankAccount bankAccount1 = new()
		{
			Id = Guid.NewGuid(),
			IBAN = "ABC123",
			Owner = user1,
			IsClosed = false,
			ClosedDate = null,
			Address = "Some test address",
			UnifiedCivilNumber = "123456789"
		};

		BankAccount bankAccount2 = new()
		{
			Id = Guid.NewGuid(),
			IBAN = "DEF456",
			Owner = user2,
			IsClosed = true,
			ClosedDate = DateTime.UtcNow,
			Address = "Some test address",
			UnifiedCivilNumber = "123456789"
		};

		_dbContext.BankSettings.Add(new()
		{
			Id = Guid.NewGuid(),
			TransactionFee = 0.5m
		});
		_dbContext.Users.Add(user1);
		_dbContext.Users.Add(user2);
		_dbContext.BankAccounts.Add(bankAccount1);
		_dbContext.BankAccounts.Add(bankAccount2);
		_dbContext.Transactions.Add(new()
		{
			Id = Guid.NewGuid(),
			Description = "Some test transaction",
			Amount = 15.00m,
			TransactionDate = DateTime.UtcNow,
			AccountFrom = bankAccount1,
			AccountFromId = bankAccount1.Id,
			AccountTo = bankAccount2,
			AccountToId = bankAccount2.Id
		});
		_dbContext.SaveChanges();
	}
}
