using Libereay_System.Entity;
using Libereay_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Libereay_System.Controllers
{
    public class UserController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(LibraryDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult BrowseBooks(string searchQuery, string category)
        {
            // Sample data for books (replace with actual data)
            var books = _context.Books
                .Where(b => string.IsNullOrEmpty(searchQuery) || b.Title.Contains(searchQuery) || b.Author.Contains(searchQuery))
                .ToList();

            // Creating the SelectList for categories
            var categories = new List<string> { "Fiction", "Science", "History" };
            ViewBag.Categories = new SelectList(categories, category); // SelectList for categories

            // Create a ViewModel and return it to the view
            var model = new BrowseBooksViewModel
            {
                SearchQuery = searchQuery,
                SelectedCategory = category,
                Books = books
            };

            return View(model);
        }


        // Book Details
        public IActionResult BookDetails(int id)
        {
            var book = _context.Books
                .FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var userId = User.Identity.Name; // Assuming User.Identity.Name stores the logged-in user ID
            var book = await _context.Books.FindAsync(bookId);

            if (book == null || book.AvailableCopies <= 0)
            {
                TempData["ErrorMessage"] = "Book is not available for borrowing.";
                return RedirectToAction("BrowseBooks");
            }

            // Create a BorrowTransaction
            var borrowTransaction = new BorrowTransaction
            {
                BookId = book.Id,
                UserId = userId,
                BorrowDate = DateTime.Now,
                ReturnDate = null // Not returned yet
            };

            // Update AvailableCopies and save changes
            book.AvailableCopies -= 1;
            _context.BorrowTransactions.Add(borrowTransaction);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book borrowed successfully!";
            return RedirectToAction("BrowseBooks");
        }

        public async Task<IActionResult> ReturnBook(int transactionId)
        {
            var transaction = await _context.BorrowTransactions
                .Include(t => t.Book)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null || transaction.ReturnDate != null)
            {
                TempData["ErrorMessage"] = "Invalid return request.";
                return RedirectToAction("BorrowingHistory");
            }

            // Mark the transaction as returned
            transaction.ReturnDate = DateTime.Now;

            // Update the book's AvailableCopies
            transaction.Book.AvailableCopies += 1;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Book returned successfully!";
            return RedirectToAction("BorrowingHistory");
        }

        public IActionResult BorrowingHistory()
        {
            var userId = User.Identity.Name; // Assuming User.Identity.Name stores the logged-in user ID
            var transactions = _context.BorrowTransactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Book)
                .OrderByDescending(t => t.BorrowDate)
                .ToList();

            return View(transactions);
        }

        public async Task<IActionResult> ProfileDetails()
        {
            var userId = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ApplicationUser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ProfileDetails");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

    }
}
