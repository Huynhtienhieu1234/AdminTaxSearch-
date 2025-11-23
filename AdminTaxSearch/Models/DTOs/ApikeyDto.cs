namespace AdminTaxSearch.Models.DTOs
{
    public class ApikeyDto
    {
        public int ApikeyId { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string Apikey { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? IssuedDate { get; set; }
        public int? UserId { get; set; }
    }

    public class CreateApikeyDto
    {
        public string SystemName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
