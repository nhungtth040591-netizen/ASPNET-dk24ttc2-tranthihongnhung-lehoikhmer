using System;
using Microsoft.AspNetCore.Mvc;
using KhmerFestival.Web.Models;

namespace KhmerFestival.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ContactInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contact = new Contact
            {
                FullName = model.FullName,
                Email = model.Email,
                Subject = model.Subject,
                Message = model.Message,
                Status = 0, // 0 = mới
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Contacts.Add(contact);
            _context.SaveChanges();

            TempData["Message"] = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm nhất có thể.";
            return RedirectToAction(nameof(Index));
        }
    }

    public class ContactInputModel
    {
        public string FullName { get; set; }
        public string Email { get; set; } 
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
