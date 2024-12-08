using Libereay_System.Entity;
using Libereay_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Libereay_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly LibraryDbContext _context;

        public AdminController(LibraryDbContext context)
        {
            _context = context;
        }

        // Manage Books
        public async Task<IActionResult> ManageBooks()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }

        [HttpGet]
        public IActionResult AddBook() => View();

        [HttpPost]
        public async Task<IActionResult> AddBook(Book book)
        {
            if (ModelState.IsValid)
            {
                book.AvailableCopies = book.TotalCopies;
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageBooks");
            }
            return View(book);
        }

        public async Task<IActionResult> EditBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageBooks");
            }
            return View(book);
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("ManageBooks");
        }

        // Display all transactions
        public async Task<IActionResult> ManageTransactions()
        {
            var transactions = await _context.BorrowTransactions
                .Include(t => t.Book)
                .Include(t => t.User)
                .ToListAsync();
            return View(transactions);
        }

        // Mark a transaction as returned
        public async Task<IActionResult> MarkAsReturned(int id)
        {
            var transaction = await _context.BorrowTransactions
                .Include(t => t.Book)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            transaction.ReturnDate = DateTime.Now;
            transaction.Book.AvailableCopies++;

            // Fine logic: Calculate the number of overdue days
            const int BorrowLimitDays = 14; // Example borrowing limit
            const decimal FinePerDay = 0.5m; // Example fine rate: $0.50 per day
            var overdueDays = (transaction.ReturnDate.Value - transaction.BorrowDate).TotalDays - BorrowLimitDays;

            // Apply fine if overdue
            transaction.FineAmount = overdueDays > 0 ? (decimal)overdueDays * FinePerDay : 0;

            _context.BorrowTransactions.Update(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageTransactions));
        }

        // Delete a transaction (optional)
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.BorrowTransactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.BorrowTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageTransactions));
        }
    }
}
