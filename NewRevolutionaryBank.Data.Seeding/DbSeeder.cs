namespace NewRevolutionaryBank.Data.Seeding;

using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Data.Models.Enums;

public static class DbSeeder
{
	public static async Task SeedBankSettingsAsync(this IApplicationBuilder app)
	{
		NrbDbContext context = GetServiceProvider(app)
			.GetRequiredService<NrbDbContext>();

		if (!context.BankSettings.Any())
		{
			await context.BankSettings.AddAsync(new()
			{
				TransactionFee = 0.10m,
				MonthlyTax = 0.50m
			});

			await context.SaveChangesAsync();
		}
	}

	public static async Task SeedRolesAsync(this IApplicationBuilder app)
	{
		RoleManager<ApplicationRole> roleManager = GetServiceProvider(app)
			.GetRequiredService<RoleManager<ApplicationRole>>();

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

	public static async Task SeedAdministratorAsync(this IApplicationBuilder app)
	{
		IServiceProvider provider = GetServiceProvider(app);

		UserManager<ApplicationUser> userManager = provider
			.GetRequiredService<UserManager<ApplicationUser>>();

		IConfigurationRoot configuration = provider
			.GetRequiredService<IConfigurationRoot>();

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

	public static async Task SeedTestUserAsync(this IApplicationBuilder app)
	{
		IServiceProvider provider = GetServiceProvider(app);

		NrbDbContext context = provider
			.GetRequiredService<NrbDbContext>();

		UserManager<ApplicationUser> userManager = provider
			.GetRequiredService<UserManager<ApplicationUser>>();

		ApplicationUser testUser = new()
		{
			Id = Guid.Parse("b310a40e-275a-4d33-8426-c89043785f5e"),
			Email = "testemail@gmail.com",
			EmailConfirmed = true,
			FirstName = "Test",
			LastName = "User",
			UserName = "TestUser",
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
			Address = "Random address for test user",
			Tier = BankAccountTier.Standard
		};

		BankAccount bankAccTwo = new()
		{
			Balance = 0.11m,
			Owner = testUser,
			OwnerId = testUser.Id,
			UnifiedCivilNumber = "7501020018",
			IBAN = "6362477910554111111811213",
			Address = "Random address for test user",
			IsClosed = true,
			ClosedDate = DateTime.UtcNow,
			Tier = BankAccountTier.Premium
		};

		BankAccount bankAccThree = new()
		{
			Balance = 92771.22m,
			Owner = testUser,
			OwnerId = testUser.Id,
			UnifiedCivilNumber = "7501020018",
			IBAN = "7161514131211101987654321",
			Address = "Random address for test user",
			IsClosed = true,
			ClosedDate = DateTime.UtcNow,
			Tier = BankAccountTier.VIP
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
		await context.BankAccounts.AddAsync(bankAccThree);

		await context.Transactions.AddAsync(transaction);

		testUser.BankAccounts.Add(bankAcc);
		testUser.BankAccounts.Add(bankAccTwo);
		testUser.BankAccounts.Add(bankAccThree);

		await context.SaveChangesAsync();
	}

	private static IServiceProvider GetServiceProvider(IApplicationBuilder app)
		=> app.ApplicationServices.CreateScope().ServiceProvider;
}
