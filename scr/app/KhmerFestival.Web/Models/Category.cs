using System;
using System.Collections.Generic;

namespace KhmerFestival.Web.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentCategoryId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public Category ParentCategory { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
