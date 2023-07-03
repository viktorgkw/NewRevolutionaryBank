namespace NewRevolutionaryBank.Data.Seeding;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using NewRevolutionaryBank.Data.Models;

public static class DbSeeder
{
	public static async Task SeedRolesAndAdministratorAsync(
		RoleManager<ApplicationRole> roleManager,
		UserManager<ApplicationUser> userManager,
		NrbDbContext context,
		IConfiguration configuration)
	{
		await SeedBankSettingsAsync(context);

		await SeedRolesAsync(roleManager);

		await SeedAdministratorAsync(configuration, userManager);

		await SeedTestUser(
			context,
			userManager,
			"TestUserOne",
			"123testuseroneemail123@gmail.com",
			Guid.Parse("c7c52461-ae6d-4ceb-2468-08db7ae8b3c2"));

		await SeedTestUser(
			context,
			userManager,
			"TestUserTwo",
			"123testusertwoemail123@gmail.com",
			Guid.Parse("e2c52461-ae6d-4ceb-2468-08db1ae8b3c2"));
	}

	private static async Task SeedBankSettingsAsync(NrbDbContext context)
	{
		if (!context.BankSettings.Any())
		{
			context.BankSettings.Add(new()
			{
				TransactionFee = 0.10m
			});

			await context.SaveChangesAsync();
		}
	}

	private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
	{
		string[] roles = new[]
		{
			"Administrator",
			"AccountHolder",
			"Guest",
		};

		foreach (string role in roles)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new ApplicationRole(role));
			}
		}
	}

	private static async Task SeedAdministratorAsync(
		IConfiguration configuration,
		UserManager<ApplicationUser> userManager)
	{
		string adminUsername = configuration["Seeding:UserName"]!;
		string adminEmail = configuration["Seeding:Email"]!;
		string adminPassword = configuration["Seeding:Password"]!;

		ApplicationUser? adminExists = await userManager.FindByEmailAsync(adminEmail);

		if (adminExists is null)
		{
			ApplicationUser adminUser = new()
			{
				UserName = adminUsername,
				Email = adminEmail,
				EmailConfirmed = true,
				FirstName = adminUsername,
				LastName = adminUsername
			};

			IdentityResult createdResult = await userManager
				.CreateAsync(adminUser, adminPassword);

			if (createdResult.Succeeded)
			{
				await userManager.AddToRoleAsync(adminUser, "Administrator");
			}
		}
	}

	private static async Task SeedTestUser(
		NrbDbContext context,
		UserManager<ApplicationUser> userManager,
		string userName,
		string email,
		Guid id)
	{
		ApplicationUser testUser = new()
		{
			Id = id,
			Email = email,
			EmailConfirmed = true,
			FirstName = "Test",
			LastName = "User",
			UserName = userName,
		};

		if (context.Users.Any(u => u.Id == testUser.Id))
		{
			return;
		}

		string testUserPassword = "@TestUser321";

		await userManager.CreateAsync(testUser, testUserPassword);

		testUser = await context.Users
			.Include(u => u.BankAccounts)
			.FirstAsync(u => u.Id == testUser.Id);

		await userManager.AddToRoleAsync(testUser, "AccountHolder");

		BankAccount bankAcc = new()
		{
			Balance = 129.37m,
			Owner = testUser,
			OwnerId = testUser.Id,
			UnifiedCivilNumber = "7501020018",
			IBAN = "1234567891011121314151617",
			Address = "Random address for test user"
		};

		BankAccount bankAccTwo = new()
		{
			Balance = 0.11m,
			Owner = testUser,
			OwnerId = testUser.Id,
			UnifiedCivilNumber = "7501020018",
			IBAN = "1234567891011121314151617",
			Address = "Random address for test user",
			IsClosed = true,
			ClosedDate = DateTime.UtcNow
		};

		Transaction transaction = new()
		{
			Amount = 12.5m,
			Description = "Seeded description.",
			TransactionDate = DateTime.UtcNow,
			AccountFrom = bankAcc,
			AccountFromId = bankAcc.Id,
			AccountTo = bankAccTwo,
			AccountToId = bankAccTwo.Id,
		};

		await context.BankAccounts.AddAsync(bankAcc);
		await context.BankAccounts.AddAsync(bankAccTwo);

		await context.Transactions.AddAsync(transaction);

		testUser.BankAccounts.Add(bankAcc);
		testUser.BankAccounts.Add(bankAccTwo);

		await context.SaveChangesAsync();
	}
}
