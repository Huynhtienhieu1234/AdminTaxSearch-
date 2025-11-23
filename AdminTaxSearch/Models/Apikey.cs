using System;
using System.Collections.Generic;

namespace AdminTaxSearch.Models;

public partial class Apikey
{
    public int ApikeyId { get; set; }

    public string SystemName { get; set; } = null!;

    public string Apikey1 { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? IssuedDate { get; set; }

    public int? RequestCount { get; set; }

    public DateTime? LastUsedDate { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
