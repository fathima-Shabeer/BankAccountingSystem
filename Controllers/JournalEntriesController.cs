using BankAccountingSystem.WebApp.Data;
using BankAccountingSystem.WebApp.Models;
using BankAccountingSystem.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BankAccountingSystem.WebApp.Controllers
{
    public class JournalEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JournalEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JournalEntries
        public async Task<IActionResult> Index()
        {
            var journalEntries = await _context.JournalEntries
                                               .Include(j => j.Lines) // Include lines if needed on index
                                               .OrderByDescending(j => j.EntryDate)
                                               .ToListAsync();
            return View(journalEntries);
        }

        // GET: JournalEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var journalEntry = await _context.JournalEntries
                .Include(j => j.Lines)
                    .ThenInclude(l => l.Account) // Include Account details for lines
                .FirstOrDefaultAsync(m => m.Id == id);

            if (journalEntry == null) return NotFound();

            return View(journalEntry);
        }


        // GET: JournalEntries/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new JournalEntryViewModel
            {
                AccountList = new SelectList(await _context.Accounts.Where(a => a.IsActive).OrderBy(a => a.AccountNumber).ToListAsync(), "Id", "Name")
                // Initialize with default date etc. if needed
                // JournalEntry = new JournalEntry { EntryDate = DateTime.Today } // Already done in model default
            };
            // Add two default empty lines if the model didn't initialize them
            if (viewModel.Lines.Count == 0)
            {
                viewModel.Lines.Add(new JournalEntryLine());
                viewModel.Lines.Add(new JournalEntryLine());
            }
            return View(viewModel);
        }

        // POST: JournalEntries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JournalEntryViewModel viewModel)
        {
            // --- Basic Validation ---
            // 1. Remove empty lines (where account is not selected or amounts are zero)
            viewModel.Lines.RemoveAll(l => l.AccountId == 0 || (l.DebitAmount == 0 && l.CreditAmount == 0));

            // 2. Check if there are at least two lines
            if (viewModel.Lines.Count < 2)
            {
                ModelState.AddModelError("", "A journal entry must have at least one debit and one credit line.");
            }

            // 3. Check if Debits equal Credits
            decimal totalDebits = viewModel.Lines.Sum(l => l.DebitAmount);
            decimal totalCredits = viewModel.Lines.Sum(l => l.CreditAmount);
            if (totalDebits != totalCredits)
            {
                ModelState.AddModelError("", $"Debits ({totalDebits:C}) must equal Credits ({totalCredits:C}).");
            }
            if (totalDebits == 0 && totalCredits == 0 && viewModel.Lines.Any()) // Avoid zero entries
            {
                ModelState.AddModelError("", "Total debits and credits cannot both be zero.");
            }

            // 4. Check individual lines (debit OR credit, not both > 0)
            foreach (var line in viewModel.Lines)
            {
                if (line.DebitAmount > 0 && line.CreditAmount > 0)
                {
                    ModelState.AddModelError($"Lines[{viewModel.Lines.IndexOf(line)}].DebitAmount", "Cannot have both Debit and Credit amount on the same line.");
                }
                if (line.AccountId == 0) // Ensure account selected
                {
                    ModelState.AddModelError($"Lines[{viewModel.Lines.IndexOf(line)}].AccountId", "Account must be selected for each line.");
                }
            }


            if (ModelState.IsValid)
            {
                // Map ViewModel to Entity
                var journalEntry = viewModel.JournalEntry; // Header info is already here
                journalEntry.Lines = viewModel.Lines; // Assign the validated lines

                // Ensure IsPosted is false initially, set PostedDate later if needed
                journalEntry.IsPosted = false;
                journalEntry.PostedDate = null;

                _context.Add(journalEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed, redisplay form
            viewModel.AccountList = new SelectList(await _context.Accounts.Where(a => a.IsActive).OrderBy(a => a.AccountNumber).ToListAsync(), "Id", "Name");
            // Ensure there are enough lines for the view template if validation fails early
            while (viewModel.Lines.Count < 2) { viewModel.Lines.Add(new JournalEntryLine()); }
            return View(viewModel);
        }

        // --- TODO: Implement Edit, Delete, Post actions ---
        // Edit/Delete are complex if entries are posted (need rules)
        // Post action would set IsPosted = true, PostedDate = DateTime.Now
    }
}