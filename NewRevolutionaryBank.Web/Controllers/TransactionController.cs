namespace NewRevolutionaryBank.Web.Controllers;

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRevolutionaryBank.Data.Models.Enums;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Transaction;

public class TransactionController : Controller
{
	private readonly ITransactionService _transactionService;

	public TransactionController(ITransactionService transactionService) =>
		_transactionService = transactionService;

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
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
			return RedirectToAction("Index", "Home");
		}
	}

	[HttpPost]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> NewTransaction(TransactionNewViewModel model)
	{
		try
		{
			TransactionNewViewModel cleanModel = await _transactionService
				.PrepareTransactionModelForUserAsync(User.Identity!.Name!);

			ModelState.Remove("SenderAccounts");

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
			return RedirectToAction("Index", "Home");
		}
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public IActionResult TransactionSuccessful()
	{
		if (TempData["RedirectedFromMethod"] is not null &&
			TempData["RedirectedFromMethod"]!.ToString() == "NewTransaction")
		{
			return View();
		}

		return RedirectToAction("Index", "Home");
	}
}
