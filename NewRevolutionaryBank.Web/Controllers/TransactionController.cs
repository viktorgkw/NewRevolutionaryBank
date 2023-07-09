namespace NewRevolutionaryBank.Web.Controllers;

using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Data.Models.Enums;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Transaction;

[Authorize(Roles = "AccountHolder")]
public class TransactionController : Controller
{
	private readonly ITransactionService _transactionService;

	public TransactionController(ITransactionService transactionService) =>
		_transactionService = transactionService;

	[HttpGet]
	public async Task<IActionResult> NewTransaction()
	{
		try
		{
			TransactionNewViewModel model = await _transactionService
				.PrepareTransactionModelForUserAsync(User.Identity!.Name!);

			return View(model);
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}
	}

	[HttpPost]
	public async Task<IActionResult> NewTransaction(TransactionNewViewModel model)
	{
		try
		{
			TransactionNewViewModel cleanModel = await _transactionService
				.PrepareTransactionModelForUserAsync(User.Identity!.Name!);

			ModelState.Remove("SenderAccounts");

			model.Description = model.Description.Trim();

			if (!ModelState.IsValid)
			{
				return View(cleanModel);
			}

			PaymentResult paymentResult = await _transactionService
				.BeginPaymentAsync(model);

			if (paymentResult != PaymentResult.Successful)
			{
				ModelState.AddModelError("",
					string.Join(" ", Regex.Split(paymentResult.ToString(), "(?<!^)(?=[A-Z])")));

				return View(cleanModel);
			}

			TempData["RedirectedFromMethod"] = "NewTransaction";
			return RedirectToAction("TransactionSuccessful", "Transaction");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}
	}

	[HttpGet]
	public IActionResult TransactionSuccessful()
	{
		if (TempData["RedirectedFromMethod"] is not null &&
			TempData["RedirectedFromMethod"]!.ToString() == "NewTransaction")
		{
			return View();
		}

		return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
	}
}
