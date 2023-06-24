namespace NewRevolutionaryBank.Data.Models.Enums;

public enum PaymentResult
{
	Successful,
	NoSelfTransactions,
	RecieverNotFound,
	SenderNotFound,
	InsufficientFunds
}
