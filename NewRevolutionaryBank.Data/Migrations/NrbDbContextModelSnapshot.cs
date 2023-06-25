﻿namespace NewRevolutionaryBank.Data.Migrations;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NewRevolutionaryBank.Data;

[DbContext(typeof(NrbDbContext))]
partial class NrbDbContextModelSnapshot : ModelSnapshot
{
	protected override void BuildModel(ModelBuilder modelBuilder)
	{
		modelBuilder
			.HasAnnotation("ProductVersion", "7.0.8")
			.HasAnnotation("Relational:MaxIdentifierLength", 128);

		SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
			{
				b.Property<int>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("int");

				SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

				b.Property<string>("ClaimType")
					.HasColumnType("nvarchar(max)");

				b.Property<string>("ClaimValue")
					.HasColumnType("nvarchar(max)");

				b.Property<Guid>("RoleId")
					.HasColumnType("uniqueidentifier");

				b.HasKey("Id");

				b.HasIndex("RoleId");

				b.ToTable("AspNetRoleClaims", (string)null);
			});

		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
			{
				b.Property<int>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("int");

				SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

				b.Property<string>("ClaimType")
					.HasColumnType("nvarchar(max)");

				b.Property<string>("ClaimValue")
					.HasColumnType("nvarchar(max)");

				b.Property<Guid>("UserId")
					.HasColumnType("uniqueidentifier");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.ToTable("AspNetUserClaims", (string)null);
			});

		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
			{
				b.Property<string>("LoginProvider")
					.HasColumnType("nvarchar(450)");

				b.Property<string>("ProviderKey")
					.HasColumnType("nvarchar(450)");

				b.Property<string>("ProviderDisplayName")
					.HasColumnType("nvarchar(max)");

				b.Property<Guid>("UserId")
					.HasColumnType("uniqueidentifier");

				b.HasKey("LoginProvider", "ProviderKey");

				b.HasIndex("UserId");

				b.ToTable("AspNetUserLogins", (string)null);
			});

		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
			{
				b.Property<Guid>("UserId")
					.HasColumnType("uniqueidentifier");

				b.Property<Guid>("RoleId")
					.HasColumnType("uniqueidentifier");

				b.HasKey("UserId", "RoleId");

				b.HasIndex("RoleId");

				b.ToTable("AspNetUserRoles", (string)null);
			});

		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
			{
				b.Property<Guid>("UserId")
					.HasColumnType("uniqueidentifier");

				b.Property<string>("LoginProvider")
					.HasColumnType("nvarchar(450)");

				b.Property<string>("Name")
					.HasColumnType("nvarchar(450)");

				b.Property<string>("Value")
					.HasColumnType("nvarchar(max)");

				b.HasKey("UserId", "LoginProvider", "Name");

				b.ToTable("AspNetUserTokens", (string)null);
			});

		modelBuilder.Entity("NewRevolutionaryBank.Data.Models.ApplicationRole", b =>
			{
				b.Property<Guid>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("uniqueidentifier");

				b.Property<string>("ConcurrencyStamp")
					.IsConcurrencyToken()
					.HasColumnType("nvarchar(max)");

				b.Property<DateTime>("CreatedOn")
					.HasColumnType("datetime2")
					.HasComment("Дата на създаване на ролята");

				b.Property<string>("Name")
					.HasMaxLength(256)
					.HasColumnType("nvarchar(256)");

				b.Property<string>("NormalizedName")
					.HasMaxLength(256)
					.HasColumnType("nvarchar(256)");

				b.HasKey("Id");

				b.HasIndex("NormalizedName")
					.IsUnique()
					.HasDatabaseName("RoleNameIndex")
					.HasFilter("[NormalizedName] IS NOT NULL");

				b.ToTable("AspNetRoles", null, t => t.HasComment("Потребителска роля"));
			});

		modelBuilder.Entity("NewRevolutionaryBank.Data.Models.ApplicationUser", b =>
			{
				b.Property<Guid>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("uniqueidentifier");

				b.Property<int>("AccessFailedCount")
					.HasColumnType("int");

				b.Property<string>("ConcurrencyStamp")
					.IsConcurrencyToken()
					.HasColumnType("nvarchar(max)");

				b.Property<DateTime>("CreatedOn")
					.HasColumnType("datetime2")
					.HasComment("Дата на създаване");

				b.Property<DateTime?>("DeletedOn")
					.HasColumnType("datetime2")
					.HasComment("Дата на изтриване");

				b.Property<string>("Email")
					.HasMaxLength(256)
					.HasColumnType("nvarchar(256)");

				b.Property<bool>("EmailConfirmed")
					.HasColumnType("bit");

				b.Property<string>("FirstName")
					.HasColumnType("nvarchar(max)")
					.HasComment("Собствено име");

				b.Property<bool>("IsDeleted")
					.HasColumnType("bit")
					.HasComment("Флаг дали профила е изтрит");

				b.Property<string>("LastName")
					.HasColumnType("nvarchar(max)")
					.HasComment("Фамилия");

				b.Property<bool>("LockoutEnabled")
					.HasColumnType("bit");

				b.Property<DateTimeOffset?>("LockoutEnd")
					.HasColumnType("datetimeoffset");

				b.Property<string>("NormalizedEmail")
					.HasMaxLength(256)
					.HasColumnType("nvarchar(256)");

				b.Property<string>("NormalizedUserName")
					.HasMaxLength(256)
					.HasColumnType("nvarchar(256)");

				b.Property<string>("PasswordHash")
					.HasColumnType("nvarchar(max)");

				b.Property<string>("PhoneNumber")
					.HasColumnType("nvarchar(max)");

				b.Property<bool>("PhoneNumberConfirmed")
					.HasColumnType("bit");

				b.Property<string>("SecurityStamp")
					.HasColumnType("nvarchar(max)");

				b.Property<bool>("TwoFactorEnabled")
					.HasColumnType("bit");

				b.Property<string>("UserName")
					.HasMaxLength(256)
					.HasColumnType("nvarchar(256)");

				b.HasKey("Id");

				b.HasIndex("NormalizedEmail")
					.HasDatabaseName("EmailIndex");

				b.HasIndex("NormalizedUserName")
					.IsUnique()
					.HasDatabaseName("UserNameIndex")
					.HasFilter("[NormalizedUserName] IS NOT NULL");

				b.ToTable("AspNetUsers", null, t => t.HasComment("Потребителски акаунт"));
			});

		modelBuilder.Entity("NewRevolutionaryBank.Data.Models.BankAccount", b =>
			{
				b.Property<Guid>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("uniqueidentifier")
					.HasComment("Уникален идентификатор");

				b.Property<string>("Address")
					.IsRequired()
					.HasMaxLength(40)
					.HasColumnType("nvarchar(40)")
					.HasComment("Адрес на потребителя");

				b.Property<decimal>("Balance")
					.HasPrecision(18, 2)
					.HasColumnType("decimal(18,2)")
					.HasComment("Салдо на сметката");

				b.Property<DateTime?>("ClosedDate")
					.HasColumnType("datetime2")
					.HasComment("Дата на закриване на сметка");

				b.Property<string>("IBAN")
					.IsRequired()
					.HasMaxLength(25)
					.HasColumnType("nvarchar(25)")
					.HasComment("ИБАН на сметката");

				b.Property<bool>("IsClosed")
					.HasColumnType("bit")
					.HasComment("Флаг дали сметката е закрита");

				b.Property<Guid>("OwnerId")
					.HasColumnType("uniqueidentifier")
					.HasComment("Уникален идентификатор на собственика на сметката");

				b.Property<string>("UnifiedCivilNumber")
					.IsRequired()
					.HasMaxLength(10)
					.HasColumnType("nvarchar(10)")
					.HasComment("ЕГН на потребителя");

				b.HasKey("Id");

				b.HasIndex("OwnerId");

				b.ToTable("BankAccounts", t => t.HasComment("Банква сметка"));
			});

		modelBuilder.Entity("NewRevolutionaryBank.Data.Models.BankSettings", b =>
			{
				b.Property<Guid>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("uniqueidentifier")
					.HasComment("Уникален идентификатор");

				b.Property<decimal>("TransactionFee")
					.HasPrecision(18, 2)
					.HasColumnType("decimal(18,2)")
					.HasComment("Такса за транзакция");

				b.HasKey("Id");

				b.ToTable("BankSettings");
			});

		modelBuilder.Entity("NewRevolutionaryBank.Data.Models.Transaction", b =>
			{
				b.Property<Guid>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("uniqueidentifier")
					.HasComment("Уникален идентификатор");

				b.Property<Guid>("AccountFromId")
					.HasColumnType("uniqueidentifier")
					.HasComment("Уникален идентификатор на предавателя");

				b.Property<Guid>("AccountToId")
					.HasColumnType("uniqueidentifier")
					.HasComment("Уникален идентификатор на получателя");

				b.Property<decimal>("Amount")
					.HasPrecision(18, 2)
					.HasColumnType("decimal(18,2)")
					.HasComment("Сума на транзакцията");

				b.Property<string>("Description")
					.IsRequired()
					.HasMaxLength(120)
					.HasColumnType("nvarchar(120)")
					.HasComment("Описание на транзакцията");

				b.Property<DateTime>("TransactionDate")
					.HasColumnType("datetime2")
					.HasComment("Дата на транзакцията");

				b.HasKey("Id");

				b.HasIndex("AccountFromId");

				b.HasIndex("AccountToId");

				b.ToTable("Transactions");
			});

		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b => b.HasOne("NewRevolutionaryBank.Data.Models.ApplicationRole", null)
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired());

		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b => b.HasOne("NewRevolutionaryBank.Data.Models.ApplicationUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired());

		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b => b.HasOne("NewRevolutionaryBank.Data.Models.ApplicationUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired());

		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
			{
				b.HasOne("NewRevolutionaryBank.Data.Models.ApplicationRole", null)
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.HasOne("NewRevolutionaryBank.Data.Models.ApplicationUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b => b.HasOne("NewRevolutionaryBank.Data.Models.ApplicationUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired());

		modelBuilder.Entity("NewRevolutionaryBank.Data.Models.BankAccount", b =>
			{
				b.HasOne("NewRevolutionaryBank.Data.Models.ApplicationUser", "Owner")
					.WithMany("BankAccounts")
					.HasForeignKey("OwnerId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.Navigation("Owner");
			});

		modelBuilder.Entity("NewRevolutionaryBank.Data.Models.Transaction", b =>
			{
				b.HasOne("NewRevolutionaryBank.Data.Models.BankAccount", "AccountFrom")
					.WithMany()
					.HasForeignKey("AccountFromId")
					.OnDelete(DeleteBehavior.Restrict)
					.IsRequired();

				b.HasOne("NewRevolutionaryBank.Data.Models.BankAccount", "AccountTo")
					.WithMany()
					.HasForeignKey("AccountToId")
					.OnDelete(DeleteBehavior.Restrict)
					.IsRequired();

				b.Navigation("AccountFrom");

				b.Navigation("AccountTo");
			});

		modelBuilder.Entity("NewRevolutionaryBank.Data.Models.ApplicationUser", b => b.Navigation("BankAccounts"));
	}
}
