namespace NewRevolutionaryBank.Services;

using Stripe;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;

public class StripeService : IStripeService
{
	public string MakePaymentAsync(StripePayment paymentInfo)
	{
		TokenCreateOptions tokenOptions = new()
		{
			Card = new TokenCardOptions
			{
				Number = paymentInfo.CardNumber,
				ExpYear = paymentInfo.ExpYear,
				ExpMonth = paymentInfo.ExpMonth,
				Cvc = paymentInfo.CVC
			}
		};

		TokenService tokenService = new();
		Token token = tokenService.Create(tokenOptions);

		ChargeCreateOptions chargeOptions = new()
		{
			Amount = paymentInfo.ChargeAmount,
			Currency = paymentInfo.Currency,
			Description = paymentInfo.Description,
			Source = token.Id
		};

		ChargeService chargeService = new();

		#pragma warning disable S1481 // Unused local variables should be removed
		#pragma warning disable IDE0059 // Unnecessary assignment of a value
		Charge charge = chargeService.Create(chargeOptions);
		#pragma warning restore IDE0059 // Unnecessary assignment of a value
		#pragma warning restore S1481 // Unused local variables should be removed

		/*
		 In development we should return this:
		 - return charge.Status
		 but since we have not fully setup Stripe.NET we won't use it.
		*/

		return "succeeded";
	}
}
