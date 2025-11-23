using AdminTaxSearch.Models;
using AdminTaxSearch.Models.DTOs;
using AdminTaxSearch.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminTaxSearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApikeyController : ControllerBase
    {
        private readonly IApikeyService _apikeyService;
        public ApikeyController(IApikeyService apikeyService) { _apikeyService = apikeyService; }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyApikeys()
        {
            int userId = int.Parse(User.FindFirst("uid")!.Value);
            var apikeys = await _apikeyService.GetByUserAsync(userId);
            return Ok(apikeys);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateApikey([FromBody] CreateApikeyDto dto)
        {
            int userId = int.Parse(User.FindFirst("uid")!.Value);
            var apikey = await _apikeyService.CreateAsync(userId, dto);
            return Ok(apikey);
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllApikeys()
        {
            var list = await _apikeyService.GetAllAsync();
            return Ok(list);
        }
    }
}
