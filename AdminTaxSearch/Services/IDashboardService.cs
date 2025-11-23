using System.Threading.Tasks;
using AdminTaxSearch.Dto;

namespace AdminTaxSearch.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsAsync();
    }
}
