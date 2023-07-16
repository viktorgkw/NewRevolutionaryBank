namespace NewRevolutionaryBank.Tests.Services;

using Microsoft.EntityFrameworkCore;
using Xunit;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Services;
using NewRevolutionaryBank.Web.ViewModels.Profile;
using NewRevolutionaryBank.Data.Models;

public class ProfileServiceTests
{
	private readonly DbContextOptions<NrbDbContext> _dbContextOptions;
	private readonly NrbDbContext _context;
	private readonly IProfileService _profileService;
	private readonly ApplicationUser customProfile = new()
	{
		FirstName = "TestFirstname",
		LastName = "TestLastname",
		UserName = "TestUsername",
		Email = "testemail@gmail.com",
		PhoneNumber = "5626532",
		CreatedOn = DateTime.Now
	};

	public ProfileServiceTests()
	{
		_dbContextOptions = new DbContextOptionsBuilder<NrbDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

		_context = new NrbDbContext(_dbContextOptions);

		_profileService = new ProfileService(_context);

		_context.Users.Add(customProfile);
		_context.SaveChanges();
	}

	[Fact]
	public async Task GetProfileDataAsync_Returns_CorrectData()
	{
		// Act
		MyProfileViewModel result = await _profileService
			.GetProfileDataAsync(customProfile.UserName!);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(customProfile.UserName, result.UserName);
		Assert.Equal(customProfile.FirstName, result.FirstName);
		Assert.Equal(customProfile.LastName, result.LastName);
		Assert.Equal(customProfile.Email, result.Email);
		Assert.Equal(customProfile.PhoneNumber, result.PhoneNumber);
		Assert.Equal(customProfile.CreatedOn, result.CreatedOn);
	}

	[Fact]
	public async Task GetProfileDataAsync_Throws_ArgumentNullException()
	{
		// Arrange
		string nonExistentUsername = "none";

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () =>
			await _profileService.GetProfileDataAsync(nonExistentUsername));
	}
}
