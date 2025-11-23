using System;
using System.Collections.Generic;

namespace AdminTaxSearch.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int RoleId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Apikey> Apikeys { get; set; } = new List<Apikey>();

    public virtual ICollection<BusinessInfo> BusinessInfos { get; set; } = new List<BusinessInfo>();

    public virtual ICollection<ChatLog> ChatLogs { get; set; } = new List<ChatLog>();

    public virtual ICollection<PersonalInfo> PersonalInfos { get; set; } = new List<PersonalInfo>();

    public virtual Role Role { get; set; } = null!;
    public virtual ICollection<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();


}
