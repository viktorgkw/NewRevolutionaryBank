namespace NewRevolutionaryBank.Web.ViewModels.BankAccount;

using NewRevolutionaryBank.Data.Models.Enums;

public class BankAccountDisplayViewModel
{
	public Guid Id { get; set; }

	public string IBAN { get; set; } = null!;

	public decimal Balance { get; set; }

    public BankAccountTier Tier { get; set; }
}
