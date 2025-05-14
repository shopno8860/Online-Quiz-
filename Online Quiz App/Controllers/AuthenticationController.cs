using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Quiz_App.Data;
using Online_Quiz_App.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace Online_Quiz_App.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthenticationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Registration Page
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        // POST: Handle Registration
        [HttpPost]
        public IActionResult Registration(User user, string ConfirmPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // Check if email already exists
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.ErrorMessage = "Email is already registered.";
                return View(user);
            }

            // Password complexity check
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{6,}$");
            if (!regex.IsMatch(user.Password))
            {
                ViewBag.ErrorMessage = "Password must be at least 6 characters, contain 1 uppercase, 1 lowercase, and 1 symbol.";
                return View(user);
            }

            // Confirm password check
            if (user.Password != ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match.";
                return View(user);
            }

            // Save user to database
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        // POST: (Optional) API-style registration
        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password, string role, string confirmPassword)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.ErrorMessage = "All fields are required.";
                return View();
            }

            // Password complexity check
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{6,}$");
            if (!regex.IsMatch(password))
            {
                ViewBag.ErrorMessage = "Password must be at least 6 characters, contain 1 uppercase, 1 lowercase, and 1 symbol.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match.";
                return View();
            }

            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.ErrorMessage = "Email is already registered.";
                return View();
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        // GET: Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Handle Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View();
            }

            // Set session values
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);

            // Redirect based on role
            if (user.Role == "Admin")
            {
                return RedirectToAction("Admin", "Quiz");
            }
            else if (user.Role == "Student")
            {
                return RedirectToAction("Home", "Quiz");
            }

            ViewBag.ErrorMessage = "Invalid role.";
            return View();
        }

        // Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
