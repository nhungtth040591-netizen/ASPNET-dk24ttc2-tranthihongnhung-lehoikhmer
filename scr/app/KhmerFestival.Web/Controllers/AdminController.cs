using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using KhmerFestival.Web.Models;

namespace KhmerFestival.Web.Controllers
{
    // TODO: khi có auth thì gắn [Authorize(Roles = "Admin,Editor")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ================== DASHBOARD ==================
        public IActionResult Dashboard()
        {
            ViewBag.FestivalCount = _context.Festivals.Count();
            ViewBag.ArticleCount = _context.Articles.Count();
            ViewBag.CommentCount = _context.Comments.Count();
            ViewBag.PendingCommentCount = _context.Comments.Count(c => c.Status == 0);
            ViewBag.ContactCount = _context.Contacts.Count();
            ViewBag.NewContactCount = _context.Contacts.Count(c => c.Status == 0);

            // Lấy một vài config cơ bản
            ViewBag.SiteName = _context.SystemConfigs
                .FirstOrDefault(c => c.ConfigKey == "SiteName")?.ConfigValue ?? "Website Lễ Hội Khmer";

            return View(); // Views/Admin/Dashboard.cshtml
        }

        // ================== SYSTEM CONFIG ==================
        [HttpGet]
        public IActionResult ConfigEdit()
        {
            var model = new SystemConfigEditViewModel
            {
                SiteName = _context.SystemConfigs
                    .FirstOrDefault(c => c.ConfigKey == "SiteName")?.ConfigValue ?? "Website Lễ Hội Khmer",
                ContactEmail = _context.SystemConfigs
                    .FirstOrDefault(c => c.ConfigKey == "ContactEmail")?.ConfigValue ?? "contact@example.com",
                FeaturedFestivalCount = int.TryParse(
                    _context.SystemConfigs.FirstOrDefault(c => c.ConfigKey == "FeaturedFestivalCount")?.ConfigValue,
                    out var n
                ) ? n : 3
            };

            return View(model); // Views/Admin/ConfigEdit.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfigEdit(SystemConfigEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetConfig("SiteName", model.SiteName, "Tên website hiển thị");
            SetConfig("ContactEmail", model.ContactEmail, "Email nhận liên hệ");
            SetConfig("FeaturedFestivalCount", model.FeaturedFestivalCount.ToString(), "Số lễ hội nổi bật trên trang chủ");

            _context.SaveChanges();

            TempData["Message"] = "Cập nhật cấu hình hệ thống thành công.";
            return RedirectToAction(nameof(Dashboard));
        }

        private void SetConfig(string key, string value, string description = null)
        {
            var config = _context.SystemConfigs.FirstOrDefault(c => c.ConfigKey == key);
            if (config == null)
            {
                config = new SystemConfig
                {
                    ConfigKey = key,
                    ConfigValue = value,
                    Description = description
                };
                _context.SystemConfigs.Add(config);
            }
            else
            {
                config.ConfigValue = value;
                if (!string.IsNullOrEmpty(description))
                    config.Description = description;
            }
        }

        // ================== FESTIVAL ==================
        public IActionResult FestivalList()
        {
            var festivals = _context.Festivals
                .Include(f => f.Location)
                .OrderByDescending(f => f.CreatedAtUtc)
                .ToList();

            return View(festivals); // Views/Admin/FestivalList.cshtml
        }

        [HttpGet]
        public IActionResult FestivalEdit(int? id)
        {
            ViewBag.Locations = _context.Locations
                .OrderBy(l => l.Name)
                .ToList();

            if (id == null)
            {
                return View(new Festival
                {
                    Status = 1,
                    IsFeatured = false
                });
            }

            var festival = _context.Festivals
                .FirstOrDefault(f => f.FestivalId == id.Value);

            if (festival == null)
                return NotFound();

            return View(festival);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FestivalEdit(Festival model, IFormFile festivalImageFile)
        {
            ViewBag.Locations = _context.Locations
                .OrderBy(l => l.Name)
                .ToList();

            if (!ModelState.IsValid)
                return View(model);

            if (model.FestivalId == 0)
            {
                // Thêm mới
                model.CreatedAtUtc = DateTime.UtcNow;

                // Nếu có upload ảnh mới
                if (festivalImageFile != null && festivalImageFile.Length > 0)
                {
                    var imgPath = SaveImage(festivalImageFile, "festivals");
                    model.ThumbnailUrl = imgPath;
                }

                _context.Festivals.Add(model);
            }
            else
            {
                // Cập nhật
                var festival = _context.Festivals
                    .FirstOrDefault(f => f.FestivalId == model.FestivalId);
                if (festival == null)
                    return NotFound();

                festival.Name = model.Name;
                festival.Slug = model.Slug;
                festival.ShortDescription = model.ShortDescription;
                festival.Content = model.Content;
                festival.StartDate = model.StartDate;
                festival.EndDate = model.EndDate;
                festival.LocationId = model.LocationId;
                festival.Meaning = model.Meaning;
                festival.IsFeatured = model.IsFeatured;
                festival.Status = model.Status;
                festival.UpdatedAtUtc = DateTime.UtcNow;

                // Nếu upload ảnh mới thì lưu file và cập nhật ThumbnailUrl
                if (festivalImageFile != null && festivalImageFile.Length > 0)
                {
                    var imgPath = SaveImage(festivalImageFile, "festivals");
                    festival.ThumbnailUrl = imgPath;
                }
            }

            _context.SaveChanges();
            TempData["Message"] = "Lưu thông tin lễ hội thành công.";
            return RedirectToAction(nameof(FestivalList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FestivalDelete(int id)
        {
            var festival = _context.Festivals.FirstOrDefault(f => f.FestivalId == id);
            if (festival != null)
            {
                _context.Festivals.Remove(festival);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(FestivalList));
        }

        // ================== CATEGORY ==================
        public IActionResult CategoryList()
        {
            var categories = _context.Categories
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.Name)
                .ToList();

            return View(categories); // Views/Admin/CategoryList.cshtml
        }

        [HttpGet]
        public IActionResult CategoryEdit(int? id)
        {
            ViewBag.Categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            if (id == null)
                return View(new Category());

            var category = _context.Categories
                .FirstOrDefault(c => c.CategoryId == id.Value);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CategoryEdit(Category model)
        {
            ViewBag.Categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            if (!ModelState.IsValid)
                return View(model);

            if (model.CategoryId == 0)
            {
                model.CreatedAtUtc = DateTime.UtcNow;
                _context.Categories.Add(model);
            }
            else
            {
                var category = _context.Categories
                    .FirstOrDefault(c => c.CategoryId == model.CategoryId);

                if (category == null)
                    return NotFound();

                category.Name = model.Name;
                category.Slug = model.Slug;
                category.ParentCategoryId = model.ParentCategoryId;
                category.Description = model.Description;
            }

            _context.SaveChanges();
            TempData["Message"] = "Lưu danh mục thành công.";
            return RedirectToAction(nameof(CategoryList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CategoryDelete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category != null)
            {
                bool hasArticles = _context.Articles.Any(a => a.CategoryId == id);
                if (hasArticles)
                {
                    TempData["Error"] = "Không thể xóa danh mục vì đang có bài viết sử dụng.";
                }
                else
                {
                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(CategoryList));
        }

        // ================== ARTICLE ==================
        public IActionResult ArticleList()
        {
            var articles = _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Festival)
                .OrderByDescending(a => a.CreatedAtUtc)
                .ToList();

            return View(articles); // Views/Admin/ArticleList.cshtml
        }

        [HttpGet]
        public IActionResult ArticleEdit(int? id)
        {
            ViewBag.Categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            ViewBag.Festivals = _context.Festivals
                .OrderBy(f => f.Name)
                .ToList();

            if (id == null)
            {
                return View(new Article
                {
                    IsPublished = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    PublishedDate = DateTime.UtcNow
                });
            }

            var article = _context.Articles
                .FirstOrDefault(a => a.ArticleId == id.Value);

            if (article == null)
                return NotFound();

            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ArticleEdit(Article model, IFormFile thumbnailFile)
        {
            ViewBag.Categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            ViewBag.Festivals = _context.Festivals
                .OrderBy(f => f.Name)
                .ToList();

            if (!ModelState.IsValid)
                return View(model);

            if (model.ArticleId == 0)
            {
                model.CreatedAtUtc = DateTime.UtcNow;
                if (model.IsPublished && model.PublishedDate == null)
                    model.PublishedDate = DateTime.UtcNow;

                // nếu upload thumbnail mới
                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    var imgPath = SaveImage(thumbnailFile, "articles");
                    model.ThumbnailUrl = imgPath;
                }

                _context.Articles.Add(model);
            }
            else
            {
                var article = _context.Articles
                    .FirstOrDefault(a => a.ArticleId == model.ArticleId);

                if (article == null)
                    return NotFound();

                article.Title = model.Title;
                article.Slug = model.Slug;
                article.Summary = model.Summary;
                article.Content = model.Content;
                article.CategoryId = model.CategoryId;
                article.FestivalId = model.FestivalId;
                article.IsPublished = model.IsPublished;
                article.PublishedDate = model.PublishedDate;
                article.UpdatedAtUtc = DateTime.UtcNow;

                // upload ảnh mới nếu có
                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    var imgPath = SaveImage(thumbnailFile, "articles");
                    article.ThumbnailUrl = imgPath;
                }
            }

            _context.SaveChanges();
            TempData["Message"] = "Lưu bài viết thành công.";
            return RedirectToAction(nameof(ArticleList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ArticleDelete(int id)
        {
            var article = _context.Articles.FirstOrDefault(a => a.ArticleId == id);
            if (article != null)
            {
                _context.Articles.Remove(article);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(ArticleList));
        }

        // ================== LOCATION ==================
        public IActionResult LocationList()
        {
            var locations = _context.Locations
                .Include(l => l.Parent)
                .OrderBy(l => l.Name)
                .ToList();

            return View(locations); // Views/Admin/LocationList.cshtml
        }

        [HttpGet]
        public IActionResult LocationEdit(int? id)
        {
            ViewBag.Locations = _context.Locations
                .OrderBy(l => l.Name)
                .ToList();

            if (id == null)
                return View(new Location());

            var location = _context.Locations
                .FirstOrDefault(l => l.LocationId == id.Value);

            if (location == null)
                return NotFound();

            return View(location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LocationEdit(Location model)
        {
            ViewBag.Locations = _context.Locations
                .OrderBy(l => l.Name)
                .ToList();

            if (!ModelState.IsValid)
                return View(model);

            if (model.LocationId == 0)
            {
                model.CreatedAtUtc = DateTime.UtcNow;
                _context.Locations.Add(model);
            }
            else
            {
                var location = _context.Locations
                    .FirstOrDefault(l => l.LocationId == model.LocationId);

                if (location == null)
                    return NotFound();

                location.Name = model.Name;
                location.ParentId = model.ParentId;
                location.Level = model.Level;
                location.Notes = model.Notes;
            }

            _context.SaveChanges();
            TempData["Message"] = "Lưu địa điểm thành công.";
            return RedirectToAction(nameof(LocationList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LocationDelete(int id)
        {
            var location = _context.Locations.FirstOrDefault(l => l.LocationId == id);
            if (location != null)
            {
                bool hasFestivals = _context.Festivals.Any(f => f.LocationId == id);
                if (hasFestivals)
                {
                    TempData["Error"] = "Không thể xóa địa điểm vì đang có lễ hội sử dụng.";
                }
                else
                {
                    _context.Locations.Remove(location);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(LocationList));
        }

        // ================== CONTACT ==================
        public IActionResult ContactList()
        {
            var contacts = _context.Contacts
                .OrderByDescending(c => c.CreatedAtUtc)
                .ToList();

            return View(contacts); // Views/Admin/ContactList.cshtml
        }

        public IActionResult ContactDetail(long id)
        {
            var contact = _context.Contacts.FirstOrDefault(c => c.ContactId == id);
            if (contact == null)
                return NotFound();

            return View(contact); // Views/Admin/ContactDetail.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContactMarkProcessed(long id)
        {
            var contact = _context.Contacts.FirstOrDefault(c => c.ContactId == id);
            if (contact == null)
                return NotFound();

            contact.Status = 1; // ví dụ: 1 = đã xử lý
            contact.ProcessedAtUtc = DateTime.UtcNow;

            _context.SaveChanges();
            return RedirectToAction(nameof(ContactDetail), new { id });
        }

        // ================== COMMENT (moderation) ==================
        public IActionResult CommentList()
        {
            var comments = _context.Comments
                .Include(c => c.Article)
                .OrderBy(c => c.Status)                         // pending lên trước
                .ThenByDescending(c => c.CreatedAtUtc)
                .ToList();

            return View(comments); // Views/Admin/CommentList.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CommentApprove(long id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.CommentId == id);
            if (comment == null)
                return NotFound();

            comment.Status = 1; // 1 = đã duyệt
            comment.UpdatedAtUtc = DateTime.UtcNow;
            _context.SaveChanges();

            return RedirectToAction(nameof(CommentList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CommentReject(long id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.CommentId == id);
            if (comment == null)
                return NotFound();

            comment.Status = 2; // 2 = từ chối
            comment.UpdatedAtUtc = DateTime.UtcNow;
            _context.SaveChanges();

            return RedirectToAction(nameof(CommentList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CommentDelete(long id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.CommentId == id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(CommentList));
        }

        // ================== HELPER LƯU ẢNH ==================
        private string SaveImage(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                return null;

            // wwwroot/uploads/{subFolder}
            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", subFolder);
            if (!Directory.Exists(uploadsRoot))
            {
                Directory.CreateDirectory(uploadsRoot);
            }

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // /uploads/{subFolder}/{fileName}
            return $"/uploads/{subFolder}/{fileName}";
        }
    }

    // ViewModel cho phần cấu hình hệ thống
    public class SystemConfigEditViewModel
    {
        public string SiteName { get; set; }
        public string ContactEmail { get; set; }
        public int FeaturedFestivalCount { get; set; }
    }
}
