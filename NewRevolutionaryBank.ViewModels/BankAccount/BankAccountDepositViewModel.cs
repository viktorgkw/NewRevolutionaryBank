namespace NewRevolutionaryBank.Web.ViewModels.BankAccount;

using System;

public class BankAccountDepositViewModel
{
    public Guid Id { get; set; }

    public string IBAN { get; set; } = null!;

    public decimal Balance { get; set; }
}
