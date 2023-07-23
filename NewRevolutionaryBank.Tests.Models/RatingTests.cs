namespace NewRevolutionaryBank.Tests.Models;

using Xunit;

using NewRevolutionaryBank.Data.Models;

public class RatingTests
{
	[Fact]
	public void Rating_Constructor_SetsId()
	{
		// Act
		Rating rating = new();

		// Assert
		Assert.NotEqual(Guid.Empty, rating.Id);
	}

	[Fact]
	public void Rating_RateValue_GetSetCorrectly()
	{
		// Arrange
		Rating rating = new();
		int expectedRateValue = 5;

		// Act
		rating.RateValue = expectedRateValue;
		int actualRateValue = rating.RateValue;

		// Assert
		Assert.Equal(expectedRateValue, actualRateValue);
	}

	[Fact]
	public void Rating_RatedBy_SetCorrectly()
	{
		// Arrange
		Rating rating = new();
		ApplicationUser expectedUser = new();

		// Act
		rating.RatedBy = expectedUser;
		ApplicationUser actualUser = rating.RatedBy;

		// Assert
		Assert.Equal(expectedUser, actualUser);
	}
}

