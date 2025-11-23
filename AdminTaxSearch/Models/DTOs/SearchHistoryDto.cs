namespace AdminTaxSearch.Models.DTOs
{
    public class SearchHistoryDto
    {
        public string InputText { get; set; } = string.Empty;
        public string? ResultText { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
