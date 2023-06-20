namespace NewRevolutionaryBank.ViewModels.BankAccount;

using System.ComponentModel.DataAnnotations;

public class BankAccountCreateViewModel
{
    [Required]
    [StringLength(10, MinimumLength = 10)]
    public string UnifiedCivilNumber { get; set; } = null!;

    [Required]
    [StringLength(40, MinimumLength = 8)]
    public string Address { get; set; } = null!;
}
