namespace AdminTaxSearch.Models.DTOs
{
    public class SearchResponse
    {
        public string Input { get; set; } = string.Empty;
        public bool Found { get; set; }
        public string Timestamp { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string? Error { get; set; }
        public string? Note { get; set; }
        public object? Data { get; set; }
        public string Source { get; set; } = "db"; // mặc định db
    }
}
