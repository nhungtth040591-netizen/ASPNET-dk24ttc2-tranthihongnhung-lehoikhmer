using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KhmerFestival.Web.Models;
using KhmerFestival.Web.Models.ViewModels;

namespace KhmerFestival.Web.Controllers
{
    public class FestivalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FestivalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // /Festival/Index?q=...
        public IActionResult Index(string q, int page = 1, int pageSize = 12)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 12;

            // Base query: chỉ lấy lễ hội đang hiển thị
            var query = _context.Festivals
                .Include(f => f.Location)
                .Where(f => f.Status == 1)
                .AsQueryable();

            // ===== TÌM KIẾM THEO TỪ KHOÁ (từ ô search ở layout) =====
            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim().ToLower();

                query = query.Where(f =>
                    f.Name.ToLower().Contains(keyword) ||
                    (!string.IsNullOrEmpty(f.ShortDescription) &&
                        f.ShortDescription.ToLower().Contains(keyword)) ||
                    (!string.IsNullOrEmpty(f.Meaning) &&
                        f.Meaning.ToLower().Contains(keyword)) ||
                    (f.Location != null &&
                        f.Location.Name.ToLower().Contains(keyword))
                );
            }

            // Tổng số dòng sau khi lọc
            var totalItems = query.Count();

            // Lấy 1 trang
            var festivals = query
                .OrderBy(f => f.StartDate ?? f.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new FestivalListViewModel
            {
                Festivals = festivals,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems,

                // Lưu lại keyword để dùng ở view (nếu cần hiển thị)
                Search = q
            };

            return View(vm);
        }

        // /Festival/Detail/{slug}
        public IActionResult Detail(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return NotFound();

            var festival = _context.Festivals
                .Include(f => f.Location)
                .Include(f => f.Articles)   // Nếu muốn load bài viết liên quan
                .FirstOrDefault(f => f.Slug == slug && f.Status == 1);

            if (festival == null)
                return NotFound();

            // Lễ hội liên quan (cùng Location)
            var relatedFestivals = _context.Festivals
                .Where(f => f.FestivalId != festival.FestivalId &&
                            f.LocationId == festival.LocationId &&
                            f.Status == 1)
                .OrderBy(f => f.StartDate ?? f.CreatedAtUtc)
                .Take(4)
                .ToList();

            ViewBag.RelatedFestivals = relatedFestivals;

            return View(festival);
        }
    }
}
