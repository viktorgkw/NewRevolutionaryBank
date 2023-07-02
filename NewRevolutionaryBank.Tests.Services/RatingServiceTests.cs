namespace NewRevolutionaryBank.Tests.Services;

using Microsoft.EntityFrameworkCore;
using Xunit;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services;

public class RatingServiceTests
{
	private readonly DbContextOptions<NrbDbContext> _dbContextOptions;

	// So why do I use "databaseName: Guid.NewGuid().ToString()" ?
	/* 
	 * Since the tests run asynchronous, when I add new Rating to the database
	 * The "GetAll_Returns_AllRatings" test fails, because other tests insert Ratings
	 * Which means when the method asserts the count, it will fail.
	 * By giving the database a unique name, each test will have its unique db.
	*/
	public RatingServiceTests() =>
		_dbContextOptions = new DbContextOptionsBuilder<NrbDbContext>()
			.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
			.Options;

	[Fact]
	public async Task GetAll_Returns_AllRatings()
	{
		// Arrange
		using NrbDbContext context = new(_dbContextOptions);

		Rating rating1 = new() { RatedBy = new ApplicationUser { UserName = "TestUser1" } };
		Rating rating2 = new() { RatedBy = new ApplicationUser { UserName = "TestUser2" } };

		context.Ratings.AddRange(rating1, rating2);

		await context.SaveChangesAsync();
		RatingService service = new(context);

		int expectedResultCount = 2;

		// Act
		List<Rating> result = await service.GetAll();

		// Assert
		Assert.Equal(expectedResultCount, result.Count);
		Assert.Contains(result, r => r.RatedBy.UserName == "TestUser1");
		Assert.Contains(result, r => r.RatedBy.UserName == "TestUser2");
	}

	[Fact]
	public async Task HasAlreadyRated_Returns_True_IfUserHasRated()
	{
		// Arrange
		using NrbDbContext context = new(_dbContextOptions);
		string userName = "TestUser";
		Rating rating = new()
		{
			RatedBy = new ApplicationUser { UserName = userName }
		};
		context.Ratings.Add(rating);
		await context.SaveChangesAsync();
		RatingService service = new(context);

		// Act
		bool result = await service.HasAlreadyRated(userName);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public async Task HasAlreadyRated_Returns_False_IfUserHasNotRated()
	{
		// Arrange
		using NrbDbContext context = new(_dbContextOptions);
		string userName = "TestUser";
		RatingService service = new(context);

		// Act
		bool result = await service.HasAlreadyRated(userName);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public async Task SendRating_AddsRatingToContext()
	{
		// Arrange
		using NrbDbContext context = new(_dbContextOptions);
		Rating rating = new();
		RatingService service = new(context);

		// Act
		await service.SendRating(rating);

		// Assert
		Assert.Contains(rating, context.Ratings);
	}
}
