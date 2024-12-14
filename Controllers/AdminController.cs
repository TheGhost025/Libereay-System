using Libereay_System.Entity;
using Libereay_System.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Libereay_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly LibraryDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,LibraryDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User); // Get the logged-in user
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return View(model);
        }

        // POST: EditProfile
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User); // Get the logged-in user
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update user properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Refresh sign-in to apply changes
            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("ProfileDetails");
        }

        // GET: ProfileDetails
        public async Task<IActionResult> ProfileDetails()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return View(user);
        }
    }
}
