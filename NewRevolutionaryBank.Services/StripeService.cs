namespace NewRevolutionaryBank.Services;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;

public class StripeService : IStripeService
{
	/// <summary>
	/// This method doesn't actually make a payment.
	/// The stripe.NET code was removed, because it only gives pointless messages and errors.
	/// </summary>
	/// <returns>succeeded</returns>
	public string MakePayment(StripePayment paymentInfo) => "succeeded";
}
