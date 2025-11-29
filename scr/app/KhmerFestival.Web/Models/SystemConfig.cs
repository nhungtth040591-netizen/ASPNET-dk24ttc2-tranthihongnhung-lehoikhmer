using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhmerFestival.Web.Models
{
    public class SystemConfig
    {
        public string ConfigKey { get; set; }      // "SiteName", "ContactEmail", ...
        public string ConfigValue { get; set; }
        public string Description { get; set; }
    }
}
