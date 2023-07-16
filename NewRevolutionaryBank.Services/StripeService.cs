namespace NewRevolutionaryBank.Services;

using Stripe;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;

public class StripeService : IStripeService
{
	/// <summary>
	/// Fake payment method.
	/// </summary>
	/// <returns>succeeded</returns>
	public string MakePayment(StripePayment paymentInfo) => "succeeded";

	//private string RealMakePayment(
	//	StripePayment paymentInfo,
	//	long chargeAmount,
	//	string currency,
	//	string description)
	//{
	//	TokenCreateOptions tokenOptions = new()
	//	{
	//		Card = new TokenCardOptions
	//		{
	//			Number = paymentInfo.CardNumber,
	//			ExpYear = paymentInfo.ExpYear,
	//			ExpMonth = paymentInfo.ExpMonth,
	//			Cvc = paymentInfo.CVC
	//		}
	//	};

	//	TokenService tokenService = new();
	//	Token token = tokenService.Create(tokenOptions);

	//	ChargeCreateOptions chargeOptions = new()
	//	{
	//		Amount = chargeAmount,
	//		Currency = currency,
	//		Description = description,
	//		Source = token.Id
	//	};

	//	ChargeService chargeService = new();
	//	Charge charge = chargeService.Create(chargeOptions);

	//	return charge.Status;
	//}
}
