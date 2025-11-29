using System;

namespace KhmerFestival.Web.Models
{
    public class AccountRole
    {
        public long AccountId { get; set; }
        public int RoleId { get; set; }
        public DateTime GrantedAtUtc { get; set; }

        public Account Account { get; set; }
        public Role Role { get; set; }
    }
}
