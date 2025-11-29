using System;

namespace KhmerFestival.Web.Models
{
    public class Contact
    {
        public long ContactId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public byte Status { get; set; }           // 0: mới, 1: đã xem, 2: đã xử lý
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? ProcessedAtUtc { get; set; }
        public string Note { get; set; }
    }
}
