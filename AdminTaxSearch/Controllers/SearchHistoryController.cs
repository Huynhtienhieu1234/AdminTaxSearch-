using AdminTaxSearch.Models.DTOs;
using AdminTaxSearch.Data;  // namespace chứa AppDbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminTaxSearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchHistoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SearchHistoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            // TODO: Lấy userId từ session / JWT token
            int? userId = 1; // tạm thời hardcode

            if (userId == null)
                return Unauthorized();

            var histories = await _context.SearchHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .Select(h => new SearchHistoryDto
                {
                    InputText = h.InputText,
                    ResultText = h.ResultText,
                    CreatedAt = h.CreatedAt
                })
                .ToListAsync();

            return Ok(histories);
        }
    }
}
