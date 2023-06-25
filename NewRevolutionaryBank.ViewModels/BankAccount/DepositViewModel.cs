namespace NewRevolutionaryBank.Web.ViewModels.BankAccount;

using NewRevolutionaryBank.Data.Models;

public class DepositViewModel
{
    public Guid DepositTo { get; set; }

    public decimal Amount { get; set; }

    public StripePayment StripePayment { get; set; } = null!;

	public List<BankAccountDepositViewModel> MyAccounts { get; set; } = null!;
}
