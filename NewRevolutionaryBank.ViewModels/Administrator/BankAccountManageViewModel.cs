namespace NewRevolutionaryBank.Web.ViewModels.Administrator;

public class BankAccountManageViewModel
{
	public Guid Id { get; set; }

	public string OwnerUsername { get; set; } = null!;

	public string IBAN { get; set; } = null!;

	public bool IsClosed { get; set; }

	public decimal Balance { get; set; }
}