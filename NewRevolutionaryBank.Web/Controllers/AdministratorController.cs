﻿namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;
using NewRevolutionaryBank.Web.ViewModels.Administrator;

[Authorize(Roles = "Administrator")]
public class AdministratorController : Controller
{
	private readonly IAdministratorService _administratorService;

	public AdministratorController(IAdministratorService administratorService)
	{
		_administratorService = administratorService;
	}

	[HttpGet]
	public async Task<IActionResult> ManageBankAccounts()
	{
		// TODO: Filters
		List<BankAccountManageViewModel> accounts = await _administratorService
			.GetAllBankAccountsAsync();

		return View(accounts);
	}

	public async Task<IActionResult> BankAccountDetails(Guid id)
	{
		try
		{
			BankAccountDetailsViewModel account = await _administratorService
				.GetBankAccountDetailsAsync(id);

			return View(account);
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("ManageBankAccounts", "Administrator");
		}
	}

	[HttpGet]
	public async Task<IActionResult> ActivateBankAccount(string id)
	{
		try
		{
			await _administratorService.ActivateBankAccountByIdAsync(id);

			return RedirectToAction("ManageBankAccounts", "Administrator");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("ManageBankAccounts", "Administrator");
		}
	}

	[HttpGet]
	public async Task<IActionResult> DeactivateBankAccount(string id)
	{
		try
		{
			await _administratorService.DeactivateBankAccountByIdAsync(id);

			return RedirectToAction("ManageBankAccounts", "Administrator");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("ManageBankAccounts", "Administrator");
		}
	}

	[HttpGet]
	public async Task<IActionResult> ManageUserProfiles()
	{
		// TODO: Filters
		List<UserProfileManageViewModel> users = await _administratorService
			.GetAllProfilesAsync();

		return View(users);
	}

	[HttpGet]
	public async Task<IActionResult> ActivateUserProfile(Guid id)
	{
		try
		{
			await _administratorService.ActivateUserProfileByIdAsync(id);

			return RedirectToAction("ManageUserProfiles", "Administrator");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("Index", "Home");
		}
		
	}

	[HttpGet]
	public async Task<IActionResult> DeactivateUserProfile(Guid id)
	{
		try
		{
			await _administratorService.DeactivateUserProfileByIdAsync(id);

			return RedirectToAction("ManageUserProfiles", "Administrator");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("Index", "Home");
		}
	}

	[HttpGet]
	public async Task<IActionResult> TransactionsList()
	{
		List<TransactionDisplayViewModel> trasactions = await _administratorService
			.GetAllTransactionsAsync();

		return View(trasactions);
	}
}
