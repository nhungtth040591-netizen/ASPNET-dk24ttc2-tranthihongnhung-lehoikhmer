using System;
using System.Collections.Generic;

namespace KhmerFestival.Web.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public string Name { get; set; }          // Tên tỉnh/huyện/xã
        public int? ParentId { get; set; }        // Địa điểm cha
        public byte? Level { get; set; }          // 1: Tỉnh, 2: Huyện, 3: Xã (tuỳ dùng)
        public string Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public Location Parent { get; set; }
        public ICollection<Location> Children { get; set; } = new List<Location>();
        public ICollection<Festival> Festivals { get; set; } = new List<Festival>();
    }
}
