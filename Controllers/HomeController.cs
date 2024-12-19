using Libereay_System.Entity;
using Libereay_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Libereay_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryDbContext _context;

        public HomeController(LibraryDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminDashboard");
            }
            else if (User.IsInRole("User"))
            {
                return RedirectToAction("UserDashboard");
            }
            return RedirectToAction("AccessDenied", "Account");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            ViewData["Title"] = "Admin Dashboard";

            var dashboardData = new
            {
                TotalBooks = _context.Books.Count(),
                BorrowedBooks = _context.BorrowTransactions.Count(t => t.ReturnDate == null),
                OverdueBooks = _context.BorrowTransactions.Count(t => t.ReturnDate == null && (t.BorrowDate.AddDays(14) < DateTime.Now))
            };

            return View(dashboardData);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> UserDashboardAsync()
        {
            var userEmail = User.Identity.Name; // Assuming User.Identity.Name stores the logged-in user ID
            var userId = _context.Users.FirstOrDefault(u => u.UserName == userEmail).Id;

            // Get featured books (you can modify this to fit your business logic)
            var featuredBooks = await _context.Books.Take(6).ToListAsync(); // Example: Get the first 6 books

            // Get borrowed books for the user
            var borrowedBooks = await _context.BorrowTransactions
                .Where(t => t.UserId == userId && t.ReturnDate == null) // Only borrow transactions where the book is not yet returned
                .Include(t => t.Book)
                .ToListAsync();

            var model = new UserDashboardViewModel
            {
                FeaturedBooks = featuredBooks,
                BorrowedBooks = borrowedBooks
            };

            return View(model);
        }
    }
}
