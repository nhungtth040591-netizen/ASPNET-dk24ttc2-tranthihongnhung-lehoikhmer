using Microsoft.AspNetCore.Mvc;
using KhmerFestival.Web.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KhmerFestival.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== LOGIN =====================

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var account = _context.Accounts.FirstOrDefault(a => a.Email == model.Email && a.IsActive);
            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "Sai tài khoản hoặc mật khẩu.");
                return View(model);
            }

            if (!VerifyPassword(model.Password, account.PasswordHash, account.PasswordSalt))
            {
                ModelState.AddModelError(string.Empty, "Sai tài khoản hoặc mật khẩu.");
                return View(model);
            }

            // KHÔNG dùng Session / Cookie ở đây
            TempData["Message"] = "Đăng nhập thành công.";

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Dashboard", "Admin");
        }

        private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (storedHash == null || storedSalt == null || string.IsNullOrEmpty(password))
                return false;

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                if (computedHash.Length != storedHash.Length)
                    return false;

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                        return false;
                }
            }

            return true;
        }

        // ===================== LOGOUT =====================

        [HttpGet]
        public IActionResult Logout()
        {
            // Không cần clear session / cookie vì mình chưa dùng
            TempData["Message"] = "Bạn đã đăng xuất.";
            return RedirectToAction("Login"); // quay về trang đăng nhập
        }

        // ===================== REGISTER =====================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = _context.Accounts.FirstOrDefault(a => a.Email == model.Email);
            if (existing != null)
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký.");
                return View(model);
            }

            CreatePasswordHash(model.Password, out var passwordHash, out var passwordSalt);

            var newAccount = new Account
            {
                Email = model.Email,
                FullName = model.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsActive = true,
                CreatedAtUtc = System.DateTime.UtcNow
            };

            _context.Accounts.Add(newAccount);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        [HttpGet]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        // ===================== FORGOT PASSWORD =====================

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO: Gửi email reset mật khẩu
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
    }
}
