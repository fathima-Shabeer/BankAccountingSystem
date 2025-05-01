using BankAccountingSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountingSystem.WebApp.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; } = string.Empty; // Default value

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // Default value

        [Required]
        [Display(Name = "Account Type")]
        public AccountType AccountType { get; set; }

        [Display(Name = "Normal Balance")]
        public NormalBalance NormalBalance { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Property for Journal Entry Lines
        public virtual ICollection<JournalEntryLine>? JournalEntryLines { get; set; }

        // Calculate Normal Balance based on Type (simplified example)
        [NotMapped] // This logic shouldn't be directly mapped to DB
        public NormalBalance CalculatedNormalBalance =>
            AccountType == AccountType.Asset || AccountType == AccountType.Expense
                ? NormalBalance.Debit
                : NormalBalance.Credit;
    }
}