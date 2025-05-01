using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BankAccountingSystem.Models;
using BankAccountingSystem.WebApp.Data;
using BankAccountingSystem.WebApp.Models;

namespace BankAccountingSystem.WebApp.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            // Order accounts for better display
            return View(await _context.Accounts.OrderBy(a => a.AccountNumber).ToListAsync());
        }

        // GET: Accounts/Details/5
       

        // GET: Accounts/Create
        public IActionResult Create()
        {
            // Provide dropdown options for Enums if needed (can also use Tag Helpers)
            ViewData["AccountType"] = new SelectList(Enum.GetValues(typeof(AccountType)));
            ViewData["NormalBalance"] = new SelectList(Enum.GetValues(typeof(NormalBalance)));
            return View();
        }

        // POST: Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccountNumber,Name,AccountType,NormalBalance,Description,IsActive")] Account account)
        {
            // Basic validation: check if account number already exists
            if (_context.Accounts.Any(a => a.AccountNumber == account.AccountNumber))
            {
                ModelState.AddModelError("AccountNumber", "Account number already exists.");
            }

            // You might want to enforce setting the 'NormalBalance' based on 'AccountType' here or in the model
            // account.NormalBalance = account.CalculatedNormalBalance; // Example

            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountType"] = new SelectList(Enum.GetValues(typeof(AccountType)), account.AccountType);
            ViewData["NormalBalance"] = new SelectList(Enum.GetValues(typeof(NormalBalance)), account.NormalBalance);
            return View(account);
        }

        // ... (Edit, Delete GET/POST actions - Generated code) ...

    }
}