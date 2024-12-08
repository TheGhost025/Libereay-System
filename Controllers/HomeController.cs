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
        public IActionResult UserDashboard()
        {
            ViewData["Title"] = "User Dashboard";
            return View();
        }
    }
}
