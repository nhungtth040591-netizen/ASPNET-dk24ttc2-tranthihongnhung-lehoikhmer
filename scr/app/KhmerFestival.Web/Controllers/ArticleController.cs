using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhmerFestival.Web.Models;
using KhmerFestival.Web.Models.ViewModels;

namespace KhmerFestival.Web.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============ DANH SÁCH BÀI VIẾT ============
        // /Article/Index?categoryId=...&festivalId=...&page=1
        public IActionResult Index(
            int? categoryId,
            int? festivalId,
            int page = 1,
            int pageSize = 9)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 9;

            // Base query: chỉ lấy bài đã xuất bản
            var query = _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Festival)
                .Where(a => a.IsPublished)
                .AsQueryable();

            // Lọc theo CategoryId
            if (categoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            // Lọc theo FestivalId
            if (festivalId.HasValue)
            {
                query = query.Where(a => a.FestivalId == festivalId.Value);
            }

            var totalItems = query.Count();

            var articles = query
                .OrderByDescending(a => a.PublishedDate ?? a.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            var festivals = _context.Festivals
                .Where(f => f.Status == 1)
                .OrderBy(f => f.Name)
                .ToList();

            var model = new ArticleListViewModel
            {
                Articles = articles,

                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems,

                Search = null,              // vì không dùng search nữa
                CategoryId = categoryId,
                FestivalId = festivalId,

                Categories = categories,
                Festivals = festivals
            };

            return View(model);
        }

        // ============ CHI TIẾT BÀI VIẾT ============
        public IActionResult Detail(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return NotFound();

            var article = _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Festival)
                .FirstOrDefault(a => a.Slug == slug && a.IsPublished);

            if (article == null)
                return NotFound();

            var comments = _context.Comments
                .Include(c => c.Account)
                .Where(c => c.ArticleId == article.ArticleId && c.Status == 1)
                .OrderByDescending(c => c.CreatedAtUtc)
                .ToList();

            // Bài viết liên quan cùng category
            var relatedArticles = _context.Articles
                .Where(a => a.IsPublished
                            && a.ArticleId != article.ArticleId
                            && a.CategoryId == article.CategoryId)
                .OrderByDescending(a => a.PublishedDate ?? a.CreatedAtUtc)
                .Take(5)
                .ToList();

            // Bài viết nổi bật khác (ví dụ: mới nhất, khác bài hiện tại)
            var featuredArticles = _context.Articles
                .Where(a => a.IsPublished && a.ArticleId != article.ArticleId)
                .OrderByDescending(a => a.PublishedDate ?? a.CreatedAtUtc)
                .Take(5)
                .ToList();

            ViewBag.FeaturedArticles = featuredArticles;

            var detailVm = new ArticleDetailViewModel
            {
                Article = article,
                Comments = comments,
                RelatedArticles = relatedArticles
            };

            return View(detailVm);
        }

        // ============ GỬI BÌNH LUẬN ============
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PostComment(int articleId, string fullName, string email, string content)
        {
            var article = _context.Articles
                .FirstOrDefault(a => a.ArticleId == articleId && a.IsPublished);

            if (article == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Nội dung bình luận không được để trống.";
                return RedirectToAction(nameof(Detail), new { slug = article.Slug });
            }

            var comment = new Comment
            {
                ArticleId = article.ArticleId,
                AccountId = null,
                FullName = string.IsNullOrWhiteSpace(fullName) ? "Khách" : fullName,
                Email = email,
                Content = content,
                Status = 0, // chờ duyệt
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            TempData["Message"] = "Bình luận của bạn đã được gửi và chờ kiểm duyệt.";
            return RedirectToAction(nameof(Detail), new { slug = article.Slug });
        }
    }
}
