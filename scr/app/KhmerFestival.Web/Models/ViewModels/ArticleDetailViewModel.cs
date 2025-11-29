using System.Collections.Generic;
using KhmerFestival.Web.Models;

namespace KhmerFestival.Web.Models.ViewModels
{
    public class ArticleDetailViewModel
    {
        // Bài viết chính
        public Article Article { get; set; }

        // Danh sách comment đã duyệt
        public IEnumerable<Comment> Comments { get; set; }

        // Bài viết liên quan
        public IEnumerable<Article> RelatedArticles { get; set; }
    }
}
