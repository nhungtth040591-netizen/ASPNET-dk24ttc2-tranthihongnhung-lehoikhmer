using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KhmerFestival.Web.Models;

namespace KhmerFestival.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy số lượng lễ hội nổi bật từ SystemConfigs (nếu có)
            int featuredFestivalCount = 3;
            var featuredConfig = _context.SystemConfigs
                .FirstOrDefault(c => c.ConfigKey == "FeaturedFestivalCount");

            if (featuredConfig != null && int.TryParse(featuredConfig.ConfigValue, out var n))
            {
                featuredFestivalCount = n;
            }

            var today = DateTime.Today;

            // Lễ hội nổi bật
            var featuredFestivals = _context.Festivals
                .Include(f => f.Location)
                .Where(f => f.Status == 1 && f.IsFeatured)
                .OrderBy(f => f.StartDate ?? f.CreatedAtUtc)
                .Take(featuredFestivalCount)
                .ToList();

            // Lễ hội sắp diễn ra: StartDate >= hôm nay, gần nhất
            var upcomingFestivals = _context.Festivals
                .Include(f => f.Location)
                .Where(f => f.Status == 1
                            && f.StartDate.HasValue
                            && f.StartDate.Value.Date >= today)
                .OrderBy(f => f.StartDate.Value)
                .Take(6)
                .ToList();

            // Bài viết mới nhất
            var latestArticles = _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Festival)
                .Where(a => a.IsPublished)
                .OrderByDescending(a => a.PublishedDate ?? a.CreatedAtUtc)
                .Take(6)
                .ToList();

            var model = new HomeIndexViewModel
            {
                FeaturedFestivals = featuredFestivals,
                UpcomingFestivals = upcomingFestivals,
                LatestArticles = latestArticles
            };

            return View(model); // Views/Home/Index.cshtml, @model HomeIndexViewModel
        }

        public IActionResult Privacy()
        {
            // Trang "Giới thiệu" / "Chính sách" cố định
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }

    // ViewModel cho trang chủ
    public class HomeIndexViewModel
    {
        public System.Collections.Generic.List<Festival> FeaturedFestivals { get; set; }
        public System.Collections.Generic.List<Festival> UpcomingFestivals { get; set; }
        public System.Collections.Generic.List<Article> LatestArticles { get; set; }
    }
}
