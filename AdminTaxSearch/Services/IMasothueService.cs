using Microsoft.OpenApi.Services;

namespace AdminTaxSearch.Services
{
    public interface IMasothueService
    {
        Task<Dictionary<string, string>?> GetPersonalInfoAsync(string cccd);
    }
}
