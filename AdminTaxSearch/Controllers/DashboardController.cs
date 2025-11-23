using AdminTaxSearch.Dto;
using AdminTaxSearch.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminTaxSearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var stats = await _dashboardService.GetStatsAsync();
            return Ok(stats);
        }

    }
}
