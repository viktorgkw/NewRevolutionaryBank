namespace NewRevolutionaryBank.Tests.Services;

using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Services.Messaging.Contracts;
using NewRevolutionaryBank.Tests.Services.Mocks;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;
using System.Reflection;

public class BankAccountServiceTests
{
	private readonly DbContextOptions<NrbDbContext> _dbContextOptions;
	private readonly NrbDbContext _dbContext;
	private readonly BankAccountService _bankAccountService;
	private readonly Mock<UserManager<ApplicationUser>> _userManager;
	private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
	private readonly IStripeService _stripeService;
	private readonly IEmailSender _emailSender;

	public BankAccountServiceTests()
	{
		_dbContextOptions = new DbContextOptionsBuilder<NrbDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_dbContext = new NrbDbContext(_dbContextOptions);

		_userManager = new(
			new Mock<IUserStore<ApplicationUser>>().Object,
			new Mock<IOptions<IdentityOptions>>().Object,
			new Mock<IPasswordHasher<ApplicationUser>>().Object,
			Array.Empty<IUserValidator<ApplicationUser>>(),
			Array.Empty<IPasswordValidator<ApplicationUser>>(),
			new Mock<ILookupNormalizer>().Object,
			new Mock<IdentityErrorDescriber>().Object,
			new Mock<IServiceProvider>().Object,
			new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

		_signInManager = new(
			_userManager.Object,
			new Mock<IHttpContextAccessor>().Object,
			new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
			new Mock<IOptions<IdentityOptions>>().Object,
			null, null);

		_stripeService = new StripeService();
		_emailSender = new MockEmailSender();
		_bankAccountService = new BankAccountService(
			_dbContext,
			_userManager.Object,
			_signInManager.Object,
			_emailSender,
			_stripeService);
	}

	[Fact]
	public async Task CreateAsync_ShouldThrowException_WhenUserNameIsNull()
	{
		// Arrange
		BankAccountCreateViewModel model = new()
		{
			UnifiedCivilNumber = "123456789",
			Address = "Test Address"
		};

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _bankAccountService.CreateAsync("NonExistingUserName", model));
	}

	[Fact]
	public async Task CreateAsync_ShouldThrowException_WhenAddressIsNull()
	{
		// Arrange
		ApplicationUser user = new()
		{
			UserName = "testUser"
		};

		_dbContext.Users.Add(user);
		await _dbContext.SaveChangesAsync();

		BankAccountCreateViewModel model = new()
		{
			UnifiedCivilNumber = "123456789"
		};

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _bankAccountService.CreateAsync(user.UserName, model));
	}

	[Fact]
	public async Task CreateAsync_ShouldThrowException_WhenUCNIsNull()
	{
		// Arrange
		ApplicationUser user = new()
		{
			UserName = "testUser"
		};

		_dbContext.Users.Add(user);
		await _dbContext.SaveChangesAsync();

		BankAccountCreateViewModel model = new()
		{
			Address = "Test Address"
		};

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _bankAccountService.CreateAsync(user.UserName, model));
	}

	[Fact]
	public async Task CreateAsync_ShouldNotAddNew_WhenUserHasMaxBankAccounts()
	{
		// Arrange
		ApplicationUser user = new()
		{
			UserName = "testUser"
		};

		_dbContext.Users.Add(user);
		await _dbContext.SaveChangesAsync();

		// Act
		for (int i = 1; i <= 6; i++)
		{
			await _bankAccountService.CreateAsync(user.UserName, new()
			{
				UnifiedCivilNumber = "123456789",
				Address = "Test Address"
			});
		}

		ApplicationUser foundUser = await _dbContext.Users
			.Include(u => u.BankAccounts)
			.FirstAsync(u => u.UserName == user.UserName);

		// Assert
		Assert.NotNull(foundUser.BankAccounts);
		Assert.Equal(5, foundUser.BankAccounts.Count);
	}

	[Fact]
	public async Task CreateAsync_AddsNewBankAccount()
	{
		// Arrange
		ApplicationUser user = new()
		{
			UserName = "testUser"
		};

		_dbContext.Users.Add(user);
		await _dbContext.SaveChangesAsync();

		BankAccountCreateViewModel model = new()
		{
			UnifiedCivilNumber = "123456789",
			Address = "Test Address"
		};

		// Act
		await _bankAccountService.CreateAsync(user.UserName, model);

		ApplicationUser foundUser = await _dbContext.Users
			.Include(u => u.BankAccounts)
			.FirstAsync(u => u.UserName == user.UserName);

		BankAccount acc = foundUser.BankAccounts.First();

		// Assert
		Assert.NotNull(foundUser);

		Assert.Single(foundUser.BankAccounts);
		Assert.Equal(model.UnifiedCivilNumber, acc.UnifiedCivilNumber);
		Assert.Equal(model.Address, acc.Address);

		Assert.False(await _userManager.Object.IsInRoleAsync(foundUser, "Guest"));
	}

	[Fact]
	public async Task GetAllUserAccountsAsync_ReturnsAllActiveUserAccounts()
	{
		// Arrange
		string userName = "testUser";

		ApplicationUser user = new()
		{
			UserName = userName,
			BankAccounts = new List<BankAccount>
			{
				new BankAccount()
				{
					Address = "TestAddress",
					IBAN = "TestIBAN",
					UnifiedCivilNumber = "TestUCN",
					Balance = 0.00m
				},
				new BankAccount()
				{
					Address = "TestAddress",
					IBAN = "TestIBAN",
					UnifiedCivilNumber = "TestUCN",
					Balance = 0.00m
				}
			}
		};

		_dbContext.Users.Add(user);
		await _dbContext.SaveChangesAsync();

		// Act
		List<BankAccountDisplayViewModel> result = await
			_bankAccountService.GetAllUserAccountsAsync(userName);

		// Assert
		Assert.Equal(2, result.Count);
	}

	[Fact]
	public async Task GetAllUserAccountsAsync_ReturnsEmptyList_WhenUserHasNoAccounts()
	{
		// Arrange
		string userName = "testUser";

		ApplicationUser user = new()
		{
			UserName = userName
		};

		_dbContext.Users.Add(user);
		await _dbContext.SaveChangesAsync();

		int expectedCount = 0;

		// Act
		List<BankAccountDisplayViewModel> result = await
			_bankAccountService.GetAllUserAccountsAsync(userName);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(expectedCount, result.Count);
	}

	[Fact]
	public async Task GetDetailsByIdAsync_WithValidData_ReturnsViewModel()
	{
		// Arrange
		ApplicationUser owner = new()
		{
			Id = Guid.NewGuid(),
			UserName = "testuser"
		};

		BankAccount accountOne = new()
		{
			Id = Guid.NewGuid(),
			Owner = owner,
			Address = "TestAddress1",
			IBAN = "TestIBAN1",
			UnifiedCivilNumber = "TestUCN1",
			Balance = 100,
			OwnerId = owner.Id
		};

		BankAccount accountTwo = new()
		{
			Id = Guid.NewGuid(),
			Owner = owner,
			Address = "TestAddress2",
			IBAN = "TestIBAN2",
			UnifiedCivilNumber = "TestUCN2",
			Balance = 100,
			OwnerId = owner.Id
		};

		Transaction toBankAcc = new()
		{
			Id = Guid.NewGuid(),
			Amount = 50,
			AccountFrom = accountTwo,
			AccountFromId = accountTwo.Id,
			Description = "TestTransaction1",
			AccountTo = accountOne,
			AccountToId = accountOne.Id,
			TransactionDate = DateTime.Now
		};

		Transaction fromBankAcc = new()
		{
			Id = Guid.NewGuid(),
			Amount = 50,
			AccountFrom = accountOne,
			AccountFromId = accountOne.Id,
			Description = "TestTransaction2",
			AccountTo = accountTwo,
			AccountToId = accountTwo.Id,
			TransactionDate = DateTime.Now
		};

		List<Deposit> deposits = new()
		{
			new Deposit()
			{
				CVC = "TestCVC1",
				CardNumber = "123",
				ExpMonth = "1",
				ExpYear = "2027",
				AccountTo = accountOne,
				AccountToId = accountOne.Id,
				DepositedAt = DateTime.Now
			},
			new Deposit()
			{
				CVC = "TestCVC2",
				CardNumber = "321",
				ExpMonth = "4",
				ExpYear = "2026",
				AccountTo = accountOne,
				AccountToId = accountOne.Id,
				DepositedAt = DateTime.Now
			}
		};

		_dbContext.Users.Add(owner);
		_dbContext.BankAccounts.Add(accountOne);
		_dbContext.BankAccounts.Add(accountTwo);
		_dbContext.Transactions.Add(toBankAcc);
		_dbContext.Transactions.Add(fromBankAcc);
		_dbContext.Deposits.AddRange(deposits);

		await _dbContext.SaveChangesAsync();

		// Act
		BankAccountDetailsViewModel? result = await _bankAccountService
			.GetDetailsByIdAsync(accountOne.Id, owner.UserName);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(accountOne.Id, result.Id);
		Assert.Single(result.RecievedTransactions);
		Assert.Single(result.SentTransactions);
		Assert.Equal(deposits.Count, result.Deposits.Count);
	}

	[Fact]
	public async Task CloseAccountByIdAsync_ClosesBankAccount()
	{
		// Arrange
		Guid id = Guid.NewGuid();

		BankAccount account = new()
		{
			Id = id,
			IsClosed = false,
			Address = "TestAddress",
			IBAN = "TestIBAN",
			UnifiedCivilNumber = "TestUCN"
		};

		_dbContext.BankAccounts.Add(account);
		await _dbContext.SaveChangesAsync();

		// Act
		await _bankAccountService.CloseAccountByIdAsync(id);

		// Assert
		Assert.True(account.IsClosed);
		Assert.NotNull(account.ClosedDate);
	}

	[Fact]
	public async Task CloseAccountByIdAsync_DoesNotCloseAlreadyClosedAccount()
	{
		// Arrange
		Guid id = Guid.NewGuid();

		BankAccount account = new()
		{
			Id = id,
			IsClosed = false,
			Address = "TestAddress",
			IBAN = "TestIBAN",
			UnifiedCivilNumber = "TestUCN"
		};

		_dbContext.BankAccounts.Add(account);
		await _dbContext.SaveChangesAsync();

		// Act
		await _bankAccountService.CloseAccountByIdAsync(id);

		BankAccount acc = await _dbContext.BankAccounts.FirstAsync();
		DateTime? deletedAt = acc.ClosedDate;

		await _bankAccountService.CloseAccountByIdAsync(id);
		acc = await _dbContext.BankAccounts.FirstAsync();
		DateTime? deletedAtLate = acc.ClosedDate;

		// Assert
		Assert.Equal(deletedAt, deletedAtLate);
	}

	[Fact]
	public async Task DepositAsync_MakesDepositToBankAccount()
	{
		// Arrange
		DepositViewModel model = new()
		{
			DepositTo = Guid.NewGuid(),
			Amount = 100.0m,
			StripePayment = new StripePayment
			{
				Id = Guid.NewGuid().ToString(),
				CardNumber = "123456789",
				CVC = "12345",
				ExpYear = "2026",
				ExpMonth = "4"
			}
		};

		BankAccount bankAccount = new()
		{
			Id = model.DepositTo,
			Address = "TestAddress",
			IBAN = "TestIBAN",
			UnifiedCivilNumber = "TestUCN",
			Balance = 0.00m
		};

		_dbContext.BankAccounts.Add(bankAccount);
		await _dbContext.SaveChangesAsync();

		// Act
		await _bankAccountService.DepositAsync(model);

		// Assert
		Assert.Equal(100.0m, bankAccount.Balance);
	}

	[Fact]
	public async Task DepositAsync_ShouldThrowArgumentNullException_WhenModelIsNull()
	{
		// Arrange
		BankAccount bankAccount = new()
		{
			Id = Guid.NewGuid(),
			Address = "TestAddress",
			IBAN = "TestIBAN",
			UnifiedCivilNumber = "TestUCN",
			Balance = 0.00m
		};

		_dbContext.BankAccounts.Add(bankAccount);
		await _dbContext.SaveChangesAsync();

		DepositViewModel? model = null;

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _bankAccountService.DepositAsync(model!));
	}

	[Fact]
	public async Task DepositAsync_ShouldNotMakeDeposit_WhenAmountIsLessOrEqualToZero()
	{
		// Arrange
		DepositViewModel model = new()
		{
			DepositTo = Guid.NewGuid(),
			Amount = 0.00m,
			StripePayment = new StripePayment
			{
				Id = Guid.NewGuid().ToString(),
				CardNumber = "123456789",
				CVC = "12345",
				ExpYear = "2026",
				ExpMonth = "4"
			}
		};

		BankAccount bankAccount = new()
		{
			Id = model.DepositTo,
			Address = "TestAddress",
			IBAN = "TestIBAN",
			UnifiedCivilNumber = "TestUCN",
			Balance = 0.00m
		};

		_dbContext.BankAccounts.Add(bankAccount);
		await _dbContext.SaveChangesAsync();

		// Act
		await _bankAccountService.DepositAsync(model);

		// Assert
		Assert.Equal(0.00m, bankAccount.Balance);
	}

	[Fact]
	public async Task DepositAsync_ShouldThrowArgumentNullException_WhenStripePaymentIsNull()
	{
		// Arrange
		DepositViewModel model = new()
		{
			DepositTo = Guid.NewGuid(),
			Amount = 0.00m
		};

		BankAccount bankAccount = new()
		{
			Id = model.DepositTo,
			Address = "TestAddress",
			IBAN = "TestIBAN",
			UnifiedCivilNumber = "TestUCN",
			Balance = 0.00m
		};

		_dbContext.BankAccounts.Add(bankAccount);
		await _dbContext.SaveChangesAsync();

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _bankAccountService.DepositAsync(model));
	}

	[Fact]
	public async Task IsOwner_ReturnsTrue_IfUserIsOwnerOfBankAccount()
	{
		// Arrange
		Guid id = Guid.NewGuid();
		string userName = "testUser";

		BankAccount account = new()
		{
			Id = id,
			Owner = new ApplicationUser { UserName = userName },
			Address = "TestAddress",
			IBAN = "TestIBAN",
			UnifiedCivilNumber = "TestUCN"
		};

		_dbContext.BankAccounts.Add(account);
		await _dbContext.SaveChangesAsync();

		// Act
		bool result = await _bankAccountService.IsOwner(id, userName);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public async Task IsOwner_ReturnsFalse_IfUserIsNotOwnerOfBankAccount()
	{
		// Arrange
		BankAccount account = new()
		{
			Owner = new ApplicationUser { UserName = "Josh" },
			Address = "TestAddress",
			IBAN = "TestIBAN",
			UnifiedCivilNumber = "TestUCN"
		};

		_dbContext.BankAccounts.Add(account);
		await _dbContext.SaveChangesAsync();

		// Act
		bool result = await _bankAccountService.IsOwner(account.Id, "Jeremy");

		// Assert
		Assert.False(result);
	}

	[Fact]
	public async Task CheckUserRole_AccountHolderWithNoBankAccounts_BecomesGuest()
	{
		// Arrange
		ApplicationUser user = new()
		{
			UserName = "testuser",
		};

		await _userManager.Object.CreateAsync(user);
		await _userManager.Object.AddToRoleAsync(user, "AccountHolder");

		ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new[]
		{
			new Claim(ClaimTypes.Name, user.UserName),
			new Claim(ClaimTypes.Role, "AccountHolder")
		}));

		// Act
		await CheckUserRole(claimsPrincipal);

		// Assert
		ApplicationUser? updatedUser = await _userManager.Object
			.FindByNameAsync(user.UserName);

		_userManager
			.Setup(m => m.IsInRoleAsync(updatedUser!, "AccountHolder"))
			.ReturnsAsync(false);

		bool isInAccountHolderRole = await _userManager.Object
			.IsInRoleAsync(updatedUser!, "AccountHolder");

		_userManager
			.Setup(m => m.IsInRoleAsync(updatedUser!, "Guest"))
			.ReturnsAsync(true);

		bool isInGuestRole = await _userManager.Object
			.IsInRoleAsync(updatedUser!, "Guest");

		Assert.False(isInAccountHolderRole);
		Assert.True(isInGuestRole);
	}

	[Fact]
	public async Task CheckUserRole_AccountHolderWithOpenBankAccounts_RetainsAccountHolderRole()
	{
		// Arrange
		ApplicationUser user = new()
		{
			UserName = "testuser",
		};

		await _userManager.Object.CreateAsync(user);
		await _userManager.Object.AddToRoleAsync(user, "AccountHolder");

		BankAccount account = new()
		{
			IsClosed = false
		};

		user.BankAccounts.Add(account);
		await _dbContext.SaveChangesAsync();

		ClaimsPrincipal claimsPrincipal = new(new ClaimsIdentity(new[]
		{
			new Claim(ClaimTypes.Name, user.UserName),
			new Claim(ClaimTypes.Role, "AccountHolder")
		}));

		// Act
		await CheckUserRole(claimsPrincipal);

		// Assert
		ApplicationUser? updatedUser = await _userManager.Object
			.FindByNameAsync(user.UserName);

		_userManager
			.Setup(m => m.IsInRoleAsync(updatedUser!, "Guest"))
			.ReturnsAsync(false);

		bool isInGuestRole = await _userManager.Object
			.IsInRoleAsync(updatedUser!, "Guest");

		Assert.False(isInGuestRole);
	}

	[Fact]
	public async Task CheckUserRole_NonAccountHolder_DoesNotModifyRoles()
	{
		// Arrange
		ApplicationUser user = new()
		{
			UserName = "testuser",
		};

		await _userManager.Object.CreateAsync(user);
		await _userManager.Object.AddToRoleAsync(user, "Guest");

		ClaimsPrincipal claimsPrincipal = new(
			new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(ClaimTypes.Role, "Guest")
			}));

		// Act
		await CheckUserRole(claimsPrincipal);

		// Assert
		ApplicationUser? updatedUser = await _userManager.Object
			.FindByNameAsync(user.UserName);

		_userManager
			.Setup(m => m.IsInRoleAsync(updatedUser!, "Guest"))
			.ReturnsAsync(true);

		bool isInGuestRole = await _userManager.Object
			.IsInRoleAsync(updatedUser!, "Guest");

		Assert.True(isInGuestRole);
	}

	[Fact]
	public void GenerateIBAN_Returns_StringWithLength25()
	{
		// Act
		MethodInfo method = typeof(BankAccountService)
			.GetMethod("GenerateIBAN", BindingFlags.NonPublic | BindingFlags.Static)!;

		int expectedIbanLength = 25;
		object iban = method.Invoke(_bankAccountService, null)!;

		// Assert
		Assert.NotNull(iban);
		Assert.Equal(expectedIbanLength, iban.ToString()!.Length);
	}

	[Fact]
	public void GenerateRandomIbanPart_Returns_StringWithLength10()
	{
		// Act
		MethodInfo method = typeof(BankAccountService)
			.GetMethod("GenerateRandomIbanPart", BindingFlags.NonPublic | BindingFlags.Static)!;

		int expectedIbanLength = 10;
		object ibanPart = method.Invoke(_bankAccountService, null)!;

		// Assert
		Assert.NotNull(ibanPart);
		Assert.Equal(expectedIbanLength, ibanPart.ToString()!.Length);
	}

	private async Task CheckUserRole(ClaimsPrincipal user) =>
		await _bankAccountService.CheckUserRole(user);
}
