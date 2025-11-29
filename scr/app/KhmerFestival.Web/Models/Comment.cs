using System;

namespace KhmerFestival.Web.Models
{
    public class Comment
    {
        public long CommentId { get; set; }

        public int ArticleId { get; set; }      // FK sang Article
        public long? AccountId { get; set; }    // nếu là thành viên đăng nhập

        public string FullName { get; set; }    // tên guest (nếu không login)
        public string Email { get; set; }       // email guest (optional)

        public string Content { get; set; }     // nội dung comment

        /// <summary>
        /// 0: Chờ duyệt, 1: Đã duyệt, 2: Bị từ chối
        /// </summary>
        public byte Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public Article Article { get; set; }
        public Account Account { get; set; }
    }
}
