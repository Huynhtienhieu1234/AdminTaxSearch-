using AdminTaxSearch.Models.Enums;

namespace AdminTaxSearch.Services
{
    public interface ITaxpayerTypeDetector
    {
        TaxpayerType Detect(string html);
    
    }
}
