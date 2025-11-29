using System;
using System.Collections.Generic;

namespace KhmerFestival.Web.Models
{
    public class Account
    {
        public long AccountId { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public ICollection<AccountRole> AccountRoles { get; set; } = new List<AccountRole>();
    }
}
