namespace NewRevolutionaryBank.Tests.ViewModels;

using Xunit;

using NewRevolutionaryBank.Web.ViewModels.BankAccount;

public class BankAccountDepositViewModelTests
{
	[Fact]
	public void BankAccountDepositViewModel_Id_SetCorrectly()
	{
		// Arrange
		BankAccountDepositViewModel viewModel = new();
		Guid expectedId = Guid.NewGuid();

		// Act
		viewModel.Id = expectedId;
		Guid actualId = viewModel.Id;

		// Assert
		Assert.Equal(expectedId, actualId);
	}

	[Fact]
	public void BankAccountDepositViewModel_IBAN_SetCorrectly()
	{
		// Arrange
		BankAccountDepositViewModel viewModel = new();
		string expectedIBAN = "1234567890";

		// Act
		viewModel.IBAN = expectedIBAN;
		string actualIBAN = viewModel.IBAN;

		// Assert
		Assert.Equal(expectedIBAN, actualIBAN);
	}

	[Fact]
	public void BankAccountDepositViewModel_Balance_SetCorrectly()
	{
		// Arrange
		BankAccountDepositViewModel viewModel = new();
		decimal expectedBalance = 100.50m;

		// Act
		viewModel.Balance = expectedBalance;
		decimal actualBalance = viewModel.Balance;

		// Assert
		Assert.Equal(expectedBalance, actualBalance);
	}
}
