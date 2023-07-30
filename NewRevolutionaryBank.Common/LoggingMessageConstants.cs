namespace NewRevolutionaryBank.Common;

using System;

public static class LoggingMessageConstants
{
	public static class InformationConstants
	{
		public const string SupportMessageSent = "User {0} sent a message to support.";

		public const string CurrencyRatesCached = "Currency rates just got cached! Time: {0}";

		public const string UserTriesToRateMoreThanOnce = "User {0} tried to rate us more than once at {1}.";

		public const string UnauthorizedUserTriedToRate = "User {0} tried to rate us without account at {1}.";

		public const string UserRated = "User {0} rated us at {1}.";
	}

	public static class WarningConstants
	{
		public const string NonExistingUserTriesToSendTransaction = "User tried to send transaction at {0} without profile!";

		public const string UserTriedToAccessUnexistingBankAccDetails = "User {0} tried to access an unexisting bank account details with id: {1} at {2}!";
	}

	public static class ErrorConstants
	{
		public const string UserHitAnError = "User {0} hit an error with description: {1}!";

		public const string ProfilePageError = "User {0} tried to access his profile page but was not found. Time: {1}";

		public const string UserTriedToCreateBankAccountWithInvalidData = "User {0} tried to create a bank account with invalid data at {1}.";

		public const string UserTriedToAccessHisBankAccountUnsuccessfully = "User {0} tried to access his bank accounts unsuccessfully at {1}.";

		public const string NotOwnerTriedToCloseBankAccount = "User {0} tried to close bank account with id: {1} at {2} but is not owner!";

		public const string UnauthorizedUserTriedToDeposit = "Unauthorized user tried to deposit at {0}!";
	}

	public static class CriticalConstants
	{
		public const string CurrencyRatesDontFetch = "Currency exchange rates dont fetch from the API! Time: {0}";

		public const string CriticalErrorInMethod = "Critical error ocurred in method {0} in controller {1} at {2}!";
	}
}
