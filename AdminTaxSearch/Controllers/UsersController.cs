using AdminTaxSearch.Models.DTOs;
using AdminTaxSearch.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminTaxSearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserImportDto>>> GetUsers()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<UserImportDto>> GetUser(int id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserImportDto>> CreateUser(UserImportDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = created.UserId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserImportDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
