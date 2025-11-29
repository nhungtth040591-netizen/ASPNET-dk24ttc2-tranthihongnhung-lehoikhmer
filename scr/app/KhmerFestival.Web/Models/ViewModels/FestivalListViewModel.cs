using System;
using System.Collections.Generic;

namespace KhmerFestival.Web.Models.ViewModels
{
    public class FestivalListViewModel
    {
        public IEnumerable<Festival> Festivals { get; set; }

        // Phân trang
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / PageSize);

        // Từ khoá tìm kiếm (q)
        public string Search { get; set; }

        // Nếu sau này bạn muốn filter thêm (tháng âm...), có thể bổ sung:
        // public string MonthLunar { get; set; }
    }
}
