using BankAccountingSystem.Models;
using BankAccountingSystem.WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAccountingSystem.WebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for currency/amounts
            modelBuilder.Entity<JournalEntryLine>()
                .Property(l => l.DebitAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<JournalEntryLine>()
                .Property(l => l.CreditAmount)
                .HasColumnType("decimal(18, 2)");

            // Define relationships explicitly (optional if conventions are followed)
            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(l => l.JournalEntry)
                .WithMany(j => j.Lines)
                .HasForeignKey(l => l.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting a JE deletes its lines

            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(l => l.Account)
                .WithMany(a => a.JournalEntryLines)
                .HasForeignKey(l => l.AccountId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting Account if used in JE lines

            // Seed some basic accounts (optional)
            modelBuilder.Entity<Account>().HasData(
                new Account { Id = 1, AccountNumber = "1010", Name = "Cash", AccountType = AccountType.Asset, NormalBalance = NormalBalance.Debit, IsActive = true },
                new Account { Id = 2, AccountNumber = "1210", Name = "Accounts Receivable", AccountType = AccountType.Asset, NormalBalance = NormalBalance.Debit, IsActive = true },
                new Account { Id = 3, AccountNumber = "2010", Name = "Accounts Payable", AccountType = AccountType.Liability, NormalBalance = NormalBalance.Credit, IsActive = true },
                new Account { Id = 4, AccountNumber = "3010", Name = "Common Stock", AccountType = AccountType.Equity, NormalBalance = NormalBalance.Credit, IsActive = true },
                new Account { Id = 5, AccountNumber = "4010", Name = "Service Revenue", AccountType = AccountType.Revenue, NormalBalance = NormalBalance.Credit, IsActive = true },
                new Account { Id = 6, AccountNumber = "5010", Name = "Rent Expense", AccountType = AccountType.Expense, NormalBalance = NormalBalance.Debit, IsActive = true }
            );
        }
    }
}