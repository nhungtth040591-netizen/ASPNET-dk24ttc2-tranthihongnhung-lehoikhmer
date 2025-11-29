using KhmerFestival.Web.Models;
using System;
using System.Collections.Generic;

public class Festival
{
    public int FestivalId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string ShortDescription { get; set; }
    public string Content { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? LocationId { get; set; }
    public string Meaning { get; set; }
    public bool IsFeatured { get; set; }
    public byte Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public string ThumbnailUrl { get; set; } 

    public Location Location { get; set; }
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}
