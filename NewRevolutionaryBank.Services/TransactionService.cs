namespace NewRevolutionaryBank.Services;

using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Data.Models.Enums;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Services.Messaging.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Transaction;

public class TransactionService : ITransactionService
{
	private readonly NrbDbContext _context;
	private readonly IEmailSender _emailSender;

	public TransactionService(
		NrbDbContext context,
		IEmailSender emailSender)
	{
		_context = context;
		_emailSender = emailSender;
	}

	/// <returns>Prepared model for new transaction.</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public async Task<TransactionNewViewModel> PrepareTransactionModelForUserAsync(
		string userName)
	{
		ApplicationUser? foundUser = await _context.Users
			.AsNoTracking()
			.Include(u => u.BankAccounts)
			.FirstOrDefaultAsync(user => user.UserName == userName);

		ArgumentNullException.ThrowIfNull(foundUser);

		List<TransactionSenderViewModel> userAccounts = foundUser!.BankAccounts
			.Where(ba => !ba.IsClosed)
			.Select(ba => new TransactionSenderViewModel
			{
				Id = ba.Id,
				IBAN = ba.IBAN,
				Balance = ba.Balance,
			})
			.ToList();

		return new()
		{
			SenderAccounts = userAccounts
		};
	}

	/// <returns>The payment result of the transaction.</returns>
	public async Task<PaymentResult> BeginPaymentAsync(TransactionNewViewModel model)
	{
		BankAccount? accountFrom = await _context.BankAccounts
			.FirstOrDefaultAsync(acc => acc.Id.ToString() == model.AccountFrom);

		BankAccount? accountTo = await _context.BankAccounts
			.FirstOrDefaultAsync(acc => acc.IBAN == model.AccountTo);

		if (accountFrom is null || accountFrom.IsClosed)
		{
			return PaymentResult.SenderNotFound;
		}

		if (accountTo is null || accountTo.IsClosed)
		{
			return PaymentResult.RecieverNotFound;
		}

		if (accountFrom == accountTo)
		{
			return PaymentResult.NoSelfTransactions;
		}

		ApplicationUser userFrom = await _context.Users
			.FirstAsync(u => u.BankAccounts.Any(ba => ba.Id == accountFrom.Id));

		ApplicationUser userTo = await _context.Users
			.FirstAsync(u => u.BankAccounts.Any(ba => ba.Id == accountTo.Id));

		BankSettings bankSettings = await _context.BankSettings.FirstAsync();

		if (accountFrom.Balance - model.Amount > 1)
		{
			Transaction newTransac = new()
			{
				Description = model.Description,
				Amount = model.Amount,
				TransactionDate = DateTime.UtcNow,
				AccountFrom = accountFrom,
				AccountFromId = accountFrom.Id,
				AccountTo = accountTo,
				AccountToId = accountTo.Id
			};

			await _context.Transactions.AddAsync(newTransac);

			accountFrom.Balance -= model.Amount + bankSettings.TransactionFee;
			accountTo.Balance += model.Amount;

			bankSettings.BankBalance += bankSettings.TransactionFee;

			await _context.SaveChangesAsync();

			await _emailSender.SendEmailAsync(
			userFrom.Email!,
				"NRB - Successful Transaction",
				$"You successfully sent ${model.Amount} to {userTo.UserName}!");

			await _emailSender.SendEmailAsync(
				userTo.Email!,
				"NRB - Successful Transaction Received",
				$"You just recieved ${model.Amount} from {userTo.UserName}!");

			return PaymentResult.Successful;
		}

		return PaymentResult.InsufficientFunds;
	}
}
