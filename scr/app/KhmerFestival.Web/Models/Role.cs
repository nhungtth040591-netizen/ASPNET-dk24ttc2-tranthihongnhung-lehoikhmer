using System;
using System.Collections.Generic;

namespace KhmerFestival.Web.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public ICollection<AccountRole> AccountRoles { get; set; }
    }
}
