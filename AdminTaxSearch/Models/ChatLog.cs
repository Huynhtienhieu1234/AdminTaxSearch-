using System;
using System.Collections.Generic;

namespace AdminTaxSearch.Models;

public partial class ChatLog
{
    public int ChatId { get; set; }

    public int UserId { get; set; }

    public string Question { get; set; } = null!;   

    public string? Answer { get; set; }

    public string? ChatType { get; set; }

    public int? ResponseTimeMs { get; set; }

    public bool? IsResolved { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
