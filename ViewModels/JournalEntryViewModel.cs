using BankAccountingSystem.WebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAccountingSystem.WebApp.ViewModels
{
    public class JournalEntryViewModel
    {
        public JournalEntry JournalEntry { get; set; } = new JournalEntry();

        // Use JournalEntryLine directly for simplicity here, could use a LineViewModel too
        public List<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>
        {
            new JournalEntryLine(), // Start with at least two empty lines
            new JournalEntryLine()
        };

        // For dropdowns
        public SelectList? AccountList { get; set; }
    }
}