namespace NewRevolutionaryBank.Services.Contracts;

using NewRevolutionaryBank.Data.Models;

public interface IStripeService
{
	string MakePaymentAsync(StripePayment paymentInfo);
}
