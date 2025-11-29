using System;
using System.Collections.Generic;

namespace KhmerFestival.Web.Models.ViewModels
{
    public class ArticleListViewModel
    {
        public IEnumerable<Article> Articles { get; set; }

        // Phân trang
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / PageSize);

        // Tìm kiếm
        public string Search { get; set; }

        // Filter theo category & festival
        public int? CategoryId { get; set; }
        public int? FestivalId { get; set; }

        // Dữ liệu dropdown
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Festival> Festivals { get; set; }
    }
}
