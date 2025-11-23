using AdminTaxSearch.Models;
using AdminTaxSearch.Models.Enums;
using System;
using System.Collections.Generic;

public partial class ScrapeResult
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public TaxpayerType Type { get; set; }
    public string? RawJson { get; set; }

    public virtual ICollection<PersonalInfo>? PersonalInfos { get; set; }
    public virtual ICollection<BusinessInfo>? BusinessInfos { get; set; }
}
