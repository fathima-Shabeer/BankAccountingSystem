using BankAccountingSystem.WebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankAccountingSystem.WebApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reports/GeneralLedger
        public async Task<IActionResult> GeneralLedger(int? accountId, DateTime? startDate, DateTime? endDate)
        {
            ViewData["Accounts"] = new SelectList(_context.Accounts.OrderBy(a => a.AccountNumber), "Id", "Name", accountId);
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");


            var query = _context.JournalEntryLines
                                .Include(l => l.JournalEntry) // Need JE Date
                                .Include(l => l.Account)    // Need Account Name/Number
                                .OrderBy(l => l.JournalEntry.EntryDate)
                                .ThenBy(l => l.JournalEntryId)
                                .AsQueryable(); // Start building the query

            if (accountId.HasValue)
            {
                query = query.Where(l => l.AccountId == accountId.Value);
            }
            if (startDate.HasValue)
            {
                query = query.Where(l => l.JournalEntry.EntryDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                // Include entries on the end date itself
                query = query.Where(l => l.JournalEntry.EntryDate <= endDate.Value);
            }

            var lines = await query.ToListAsync();

            // TODO: Calculate running balance (more complex)

            return View(lines);
        }

        // TODO: Add Actions for Trial Balance, Income Statement, Balance Sheet
        // These require summing up balances across accounts based on GL lines.
    }
}