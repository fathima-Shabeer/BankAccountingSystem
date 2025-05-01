using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccountingSystem.WebApp.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Entry Date")]
        public DateTime EntryDate { get; set; } = DateTime.Today;

        [Required]
        [StringLength(250)]
        public string Description { get; set; } = string.Empty; // Default value

        [StringLength(50)]
        [Display(Name = "Reference Number")]
        public string? ReferenceNumber { get; set; }

        public bool IsPosted { get; set; } = false; // Simple posting status

        [DataType(DataType.Date)]
        [Display(Name = "Posted Date")]
        public DateTime? PostedDate { get; set; }

        // Navigation Property for Lines
        public virtual ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>(); // Initialize collection

        // Calculated property (not mapped) to check balance
        [NotMapped]
        public decimal TotalDebits => Lines?.Sum(l => l.DebitAmount) ?? 0;

        [NotMapped]
        public decimal TotalCredits => Lines?.Sum(l => l.CreditAmount) ?? 0;

        [NotMapped]
        public bool IsBalanced => TotalDebits == TotalCredits;
    }
}