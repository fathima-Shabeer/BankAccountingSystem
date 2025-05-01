using BankAccountingSystem.WebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountingSystem.WebApp.Models
{
    public class JournalEntryLine
    {
        public int Id { get; set; }

        [Required]
        public int JournalEntryId { get; set; }
        public virtual JournalEntry? JournalEntry { get; set; } // Navigation property

        [Required]
        [Display(Name = "Account")]
        public int AccountId { get; set; }
        public virtual Account? Account { get; set; } // Navigation property

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Debit amount must be non-negative.")]
        [Display(Name = "Debit")]
        public decimal DebitAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Credit amount must be non-negative.")]
        [Display(Name = "Credit")]
        public decimal CreditAmount { get; set; } = 0;

        [StringLength(250)]
        public string? Description { get; set; }

        // Validation: Ensure only one of Debit or Credit has a value > 0 (or both are 0)
        [NotMapped] // Basic validation check, more robust validation needed in practice
        public bool IsDebitOrCreditValid => (DebitAmount > 0 && CreditAmount == 0) || (CreditAmount > 0 && DebitAmount == 0) || (DebitAmount == 0 && CreditAmount == 0);
    }
}