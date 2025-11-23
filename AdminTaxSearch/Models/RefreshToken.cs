using System;

namespace AdminTaxSearch.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public bool Revoked { get; set; } = false;

        public string? CreatedByIp { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
