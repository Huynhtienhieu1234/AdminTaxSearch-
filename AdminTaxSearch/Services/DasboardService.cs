using AdminTaxSearch.Data;
using AdminTaxSearch.Dto;
using AdminTaxSearch.Services;
using Microsoft.EntityFrameworkCore;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var users = await _context.Users.CountAsync();
        var apiCalls = await _context.Apikeys.SumAsync(a => (int?)a.RequestCount ?? 0);
        var searches = await _context.SearchHistories.CountAsync();
        var apiKeyCount = await _context.Apikeys.CountAsync();



        return new DashboardStatsDto
        {
            Users = users,
            ApiCalls = apiCalls,
            Searches = searches,
            ApiKeys = apiKeyCount
        };
    }
}
