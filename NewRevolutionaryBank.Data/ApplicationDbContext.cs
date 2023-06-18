namespace NewRevolutionaryBank.Data;

using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options) { }

	public override DbSet<ApplicationUser> Users { get; set; } = null!;

	public override DbSet<ApplicationRole> Roles { get; set; } = null!;

	public DbSet<BankAccount> BankAccounts { get; set; } = null!;

	public DbSet<Transaction> Transactions { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.Entity<Transaction>()
			.HasOne(t => t.AccountTo)
			.WithMany()
			.HasForeignKey(t => t.AccountToId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.Entity<BankAccount>()
			.Property(b => b.Balance)
			.HasPrecision(18, 2);

		builder.Entity<Transaction>()
			.Property(b => b.Amount)
			.HasPrecision(18, 2);

		base.OnModelCreating(builder);
	}
}