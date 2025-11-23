namespace AdminTaxSearch.Models.DTOs
{
    public class ScrapeSearchResultDto
    {
        public string Input { get; set; } = "";
        public string Timestamp { get; set; } = "";
        public bool Found { get; set; } = false;
        public string? DetailUrl { get; set; }
        public dynamic? Data { get; set; }
        public string? Error { get; set; }
        public string? Note { get; set; }
    }
}
