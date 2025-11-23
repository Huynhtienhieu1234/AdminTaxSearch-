using AdminTaxSearch.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public partial class PersonalInfo
{
    public int PersonId { get; set; }

    public int? ScrapeResultId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Idnumber { get; set; }
    public string? TaxId { get; set; }
    public DateOnly? DateOfBirth { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }
    public string? Occupation { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? UserId { get; set; }

    public virtual ScrapeResult? ScrapeResult { get; set; }

    public virtual ICollection<SearchHistory>? SearchHistories { get; set; }
}
