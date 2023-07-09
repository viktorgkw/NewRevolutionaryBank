namespace NewRevolutionaryBank.Common;

public static class ModelsConstants
{
	public static class RatingConstants
	{
		// RateValue
		public const int RateValueMin = 1;
		public const int RateValueMax = 5;
	}

	public static class UserConstants
	{
		// FirstName
		public const int FirstNameMinLength = 3;
		public const int FirstNameMaxLength = 46;

		// LastName
		public const int LastNameMinLength = 3;
		public const int LastNameMaxLength = 46;
	}

	public static class TransactionConstants
	{
		// Amount
		public const double AmountMin = 10.00;
		public const double AmountMax = 10000.00;

		// Description
		public const int DescriptionMinLength = 5;
		public const int DescriptionMaxLength = 120;
	}

	public static class DepositConstants
	{
		// Amount
		public const double AmountMin = 10.00;
		public const double AmountMax = 10000.00;

		// ExpMonth
		public const int ExpMonthMin = 1;
		public const int ExpMonthMax = 12;

		// ExpYear
		public const int ExpYearMin = 2023;
		public const int ExpYearMax = 2060;
	}

	public static class BankAccountConstants
	{
		// IBAN
		public const int IBAN_Length = 25;

		// UnifiedCivilNumber
		public const int UCN_Length = 10;

		// Address
		public const int AddressMinLength = 8;
		public const int AddressMaxLength = 40;
	}
}
