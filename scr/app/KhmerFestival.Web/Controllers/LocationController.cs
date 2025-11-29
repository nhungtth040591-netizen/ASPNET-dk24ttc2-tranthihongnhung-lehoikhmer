using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;               // <-- THÊM DÒNG NÀY
using KhmerFestival.Web.Models;
using KhmerFestival.Web.Models.ViewModels;

namespace KhmerFestival.Web.Controllers
{
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // /Location/Index?q=...&level=...&page=1
        public IActionResult Index(string q, byte? level, int page = 1, int pageSize = 9)
        {
            var query = _context.Locations
                .Include(l => l.Parent)   // dùng được Include sau khi thêm using Microsoft.EntityFrameworkCore
                .AsQueryable();

            // Nếu sau này bỏ search thì có thể xoá block này
            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim().ToLower();
                query = query.Where(l => l.Name.ToLower().Contains(keyword));
            }

            if (level.HasValue)
            {
                query = query.Where(l => l.Level == level.Value);
            }

            var totalItems = query.Count();

            var locations = query
                .OrderBy(l => l.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new LocationListViewModel
            {
                Locations = locations,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                Search = q   // nếu bỏ search hẳn có thể để null
            };

            return View(vm);
        }

        // Detail theo id
        public IActionResult Detail(int id)
        {
            var location = _context.Locations
                .Include(l => l.Parent)
                .FirstOrDefault(l => l.LocationId == id);

            if (location == null)
                return NotFound();

            return View(location); // View Detail dùng @model Location
        }
    }
}
