using System;
using System.Collections.Generic;

namespace KhmerFestival.Web.Models
{
    public class Article
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public int? CategoryId { get; set; }
        public int? FestivalId { get; set; }
        public string ThumbnailUrl { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool IsPublished { get; set; }
        public long? AuthorId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public Category Category { get; set; }
        public Festival Festival { get; set; }
        public Account Author { get; set; }

        // NEW: danh sách comment của bài viết
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
