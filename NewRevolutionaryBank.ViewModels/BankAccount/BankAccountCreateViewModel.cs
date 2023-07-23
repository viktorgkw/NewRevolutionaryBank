namespace NewRevolutionaryBank.Web.ViewModels.BankAccount;

using System.ComponentModel.DataAnnotations;

using NewRevolutionaryBank.Data.Models.Enums;

public class BankAccountCreateViewModel
{
	[Required]
	[StringLength(10, MinimumLength = 10)]
	public string UnifiedCivilNumber { get; set; } = null!;

	[Required]
	[StringLength(40, MinimumLength = 8)]
	public string Address { get; set; } = null!;

	[Required]
	public BankAccountTier Tier { get; set; }

	[Required]
	public HashSet<BankAccountTier> Tiers { get; set; } = null!;
}
