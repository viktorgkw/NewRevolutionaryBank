namespace NewRevolutionaryBank.Tests.ViewModels;

using Xunit;

using NewRevolutionaryBank.Web.ViewModels.Home;

public class ErrorViewModelTests
{
	[Fact]
	public void ErrorViewModel_Title_SetCorrectly()
	{
		// Arrange
		ErrorViewModel errorViewModel = new();
		string expectedTitle = "Error";

		// Act
		errorViewModel.Title = expectedTitle;
		string actualTitle = errorViewModel.Title;

		// Assert
		Assert.Equal(expectedTitle, actualTitle);
	}

	[Fact]
	public void ErrorViewModel_Description_SetCorrectly()
	{
		// Arrange
		ErrorViewModel errorViewModel = new();
		string expectedDescription = "An error occurred.";

		// Act
		errorViewModel.Description = expectedDescription;
		string actualDescription = errorViewModel.Description;

		// Assert
		Assert.Equal(expectedDescription, actualDescription);
	}

	[Fact]
	public void ErrorViewModel_IsNotFound_SetCorrectly()
	{
		// Arrange
		ErrorViewModel errorViewModel = new();
		bool expectedIsNotFound = true;

		// Act
		errorViewModel.IsNotFound = expectedIsNotFound;
		bool actualIsNotFound = errorViewModel.IsNotFound;

		// Assert
		Assert.Equal(expectedIsNotFound, actualIsNotFound);
	}
}
