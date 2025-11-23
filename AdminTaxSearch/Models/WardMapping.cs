using System;
using System.Collections.Generic;

namespace AdminTaxSearch.Models;

public partial class WardMapping
{
    public int Id { get; set; }

    public string? OldWardCode { get; set; }

    public string? OldWardName { get; set; }

    public string? OldDistrictName { get; set; }

    public string? OldProvinceName { get; set; }

    public string? NewWardCode { get; set; }

    public string? NewWardName { get; set; }

    public string? NewProvinceName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }
}
