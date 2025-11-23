namespace AdminTaxSearch.Models.DTOs
{
    public class TaxInfo
    {
        public string TaxId { get; set; } = "";
        public string? Idnumber { get; set; }

        public string Name { get; set; } = "";
        public string Type { get; set; } = ""; 
        public string? Representative { get; set; }
        public string? Address { get; set; }
        public string? Status { get; set; }
        public DateOnly? DateOfBirth { get; set; }  
        public DateOnly? FoundingDate { get; set; } 
    }
}
