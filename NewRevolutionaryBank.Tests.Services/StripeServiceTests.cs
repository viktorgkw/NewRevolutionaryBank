namespace NewRevolutionaryBank.Tests.Services;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services;

using Xunit;

public class StripeServiceTests
{
	[Fact]
	public void MakePayment_Should_Return_Succeeded()
	{
		// Arrange
		StripeService service = new();
		StripePayment payment = new()
		{
			Id = Guid.NewGuid().ToString(),
			CardNumber = "123456789",
			CVC = "12345",
			ExpYear = "2026",
			ExpMonth = "4"
		};
		string expectedResult = "succeeded";

		// Act
		string result = service.MakePayment(payment);

		// Assert
		Assert.Equal(expectedResult, result);
	}
}
