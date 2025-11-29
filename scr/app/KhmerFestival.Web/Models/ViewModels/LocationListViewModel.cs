using System;
using System.Collections.Generic;

namespace KhmerFestival.Web.Models.ViewModels
{
    public class LocationListViewModel
    {
        /// <summary>
        /// Danh sách địa điểm hiển thị trên trang
        /// </summary>
        public IEnumerable<Location> Locations { get; set; }

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Số bản ghi mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số bản ghi (sau khi filter/search)
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages => PageSize > 0
            ? (int)Math.Ceiling((decimal)TotalItems / PageSize)
            : 0;

        /// <summary>
        /// Từ khoá tìm kiếm (theo tên, ghi chú, ...)
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Filter theo cấp (1: tỉnh, 2: huyện, 3: xã - tuỳ bạn dùng)
        /// </summary>
        public byte? Level { get; set; }

        /// <summary>
        /// Filter theo địa điểm cha (ví dụ: chỉ xem các huyện thuộc 1 tỉnh)
        /// </summary>
        public int? ParentId { get; set; }
    }
}
