using AdminTaxSearch.Models;
using System.ComponentModel.DataAnnotations;

public partial class BusinessInfo
{
    public int BusinessId { get; set; }
    public int? ScrapeResultId { get; set; }
    public string BusinessName { get; set; } = null!;
    public string TaxId { get; set; } = null!;
    public string? HeadOffice { get; set; }
    public string? Representative { get; set; }
    public string? BusinessField { get; set; }
    public string? BusinessType { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }
    public DateOnly? FoundingDate { get; set; }
    public string? Status { get; set; }

    public virtual ScrapeResult? ScrapeResult { get; set; }
    public virtual ICollection<SearchHistory>? SearchHistories { get; set; }
}
