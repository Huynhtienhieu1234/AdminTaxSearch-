using AdminTaxSearch.Models.Enums;
using AdminTaxSearch.Services;

public class TaxpayerTypeDetector : ITaxpayerTypeDetector
{
    public TaxpayerType Detect(string html)
    {
        if (html.Contains("Cá nhân", StringComparison.OrdinalIgnoreCase) ||
            html.Contains("Ngày sinh") ||
            html.Contains("CMND") ||
            html.Contains("CCCD"))
        {
            return TaxpayerType.Personal;
        }

        return TaxpayerType.Business;
    }
}
