﻿namespace NewRevolutionaryBank.Web.ViewModels.BankAccount;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Data.Models.Enums;

public class BankAccountDetailsViewModel
{
	public Guid Id { get; set; }

	public string IBAN { get; set; } = null!;

	public decimal Balance { get; set; }

	public string UnifiedCivilNumber { get; set; } = null!;

	public string Address { get; set; } = null!;

    public BankAccountTier Tier { get; set; }

    public HashSet<Transaction> SentTransactions { get; set; } = null!;

	public HashSet<Transaction> RecievedTransactions { get; set; } = null!;

    public HashSet<Deposit> Deposits { get; set; } = null!;
}
