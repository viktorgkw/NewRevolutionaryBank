namespace NewRevolutionaryBank.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewRevolutionaryBank.Data.Models;

public class NrbDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
	public NrbDbContext(DbContextOptions<NrbDbContext> options)
		: base(options) { }

	public override DbSet<ApplicationUser> Users { get; set; } = null!;

	public override DbSet<ApplicationRole> Roles { get; set; } = null!;

	public DbSet<BankSettings> BankSettings { get; set; } = null!;

	public DbSet<BankAccount> BankAccounts { get; set; } = null!;

	public DbSet<Transaction> Transactions { get; set; } = null!;

	public DbSet<Deposit> Deposits { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.Entity<Deposit>(options => options
				.Property(b => b.Amount)
				.HasPrecision(18, 2));

		builder.Entity<BankSettings>(options => options
				.Property(b => b.TransactionFee)
				.HasPrecision(18, 2));

		builder.Entity<BankAccount>()
			.Property(b => b.Balance)
			.HasPrecision(18, 2);

		builder.Entity<Transaction>(options =>
		{
			options
				.Property(b => b.Amount)
				.HasPrecision(18, 2);

			options
				.HasOne(t => t.AccountTo)
				.WithMany()
				.HasForeignKey(t => t.AccountToId)
				.OnDelete(DeleteBehavior.Restrict);

			options
				.HasOne(t => t.AccountFrom)
				.WithMany()
				.HasForeignKey(t => t.AccountFromId)
				.OnDelete(DeleteBehavior.Restrict);
		});

		base.OnModelCreating(builder);
	}
}