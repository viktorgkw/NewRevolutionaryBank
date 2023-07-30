namespace NewRevolutionaryBank.Web.Controllers;

using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Data.Models.Enums;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Transaction;

using static NewRevolutionaryBank.Common.LoggingMessageConstants;

[Authorize(Roles = "AccountHolder")]
public class TransactionController : Controller
{
	private readonly ITransactionService _transactionService;
	private readonly ILogger<TransactionController> _logger;

	public TransactionController(
		ITransactionService transactionService,
		ILogger<TransactionController> logger)
	{
		_transactionService = transactionService;
		_logger = logger;
	}

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
			_logger.LogWarning(string.Format(
				WarningConstants.NonExistingUserTriesToSendTransaction,
				DateTime.UtcNow));

			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "User not found!",
					description = "There might be problem with your profile! Contact our support the details for help!",
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
			_logger.LogWarning(string.Format(
				WarningConstants.NonExistingUserTriesToSendTransaction,
				DateTime.UtcNow));

			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "User not found!",
					description = "There might be problem with your profile! Contact our support the details for help!",
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
					title = "404",
					description = "The page you are trying to access does not exist!",
					isNotFound = true
				}
			);
	}
}
