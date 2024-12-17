using Libereay_System.Entity;
using Libereay_System.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Libereay_System.Controllers
{
    public class BookController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(LibraryDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // Manage Books
        public async Task<IActionResult> ManageBooks()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }

        // Action to view the details of a book
        public async Task<IActionResult> BookDetails(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpGet]
        public IActionResult AddBook()
        {
            ViewData["Title"] = "Create Book";
            ViewData["Action"] = "AddBook";

            return View(new Book());
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(Book book)
        {
            if (ModelState.IsValid)
            {
                // Handle image file upload
                if (book.ImageFile != null)
                {
                    // Generate a unique file name for the image
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(book.ImageFile.FileName);
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    // Ensure the upload directory exists
                    Directory.CreateDirectory(uploadPath);

                    // Full path to save the image
                    string filePath = Path.Combine(uploadPath, fileName);

                    // Save the image file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await book.ImageFile.CopyToAsync(fileStream);
                    }

                    // Set the ImagePath property to the relative path
                    book.ImagePath = "/uploads/" + fileName;
                }
                book.AvailableCopies = book.TotalCopies;
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("BookDetails", "Book", new { id = book.Id });
            }
            ViewData["Title"] = "Create Book";
            ViewData["Action"] = "AddBook";
            return View(book);
        }

        public async Task<IActionResult> EditBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            ViewData["Title"] = "Edit Book";
            ViewData["Action"] = "EditBook";
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(Book book)
        {
            if (ModelState.IsValid || book.ImagePath != null)
            {
                // Handle image file upload if provided
                if (book.ImageFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(book.ImageFile.FileName);
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    Directory.CreateDirectory(uploadPath);
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await book.ImageFile.CopyToAsync(fileStream);
                    }

                    book.ImagePath = "/uploads/" + fileName;
                }
                int AvailableCopies = book.AvailableCopies;
                AvailableCopies += book.TotalCopies - book.AvailableCopies;
                // Only update if both AvailableCopies and TotalCopies are valid (non-negative)
                if (AvailableCopies >= 0)
                {
                    book.AvailableCopies = AvailableCopies;
                    _context.Books.Update(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("BookDetails", "Book", new { id = book.Id });
                }
                else
                {
                    ModelState.AddModelError("TotalCopies", "Total copies is not suitable.");
                }
            }
            ViewData["Title"] = "Edit Book";
            ViewData["Action"] = "EditBook";
            return View(book);
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            // Delete the book's image if it exists
            if (!string.IsNullOrEmpty(book.ImagePath))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, book.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
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
