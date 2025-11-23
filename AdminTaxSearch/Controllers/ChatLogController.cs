using AdminTaxSearch.Models;
using AdminTaxSearch.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminTaxSearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatLogController : ControllerBase
    {
        private readonly IChatLogService _service;

        public ChatLogController(IChatLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _service.GetAllAsync();
            return Ok(logs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _service.GetByIdAsync(id);
            if (log == null) return NotFound();
            return Ok(log);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChatLog chatLog)
        {
            if (chatLog == null) return BadRequest();
            var created = await _service.CreateAsync(chatLog);
            return CreatedAtAction(nameof(GetById), new { id = created.ChatId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ChatLog chatLog)
        {
            if (chatLog == null) return BadRequest();
            var updated = await _service.UpdateAsync(id, chatLog);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // Optional: get logs by user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var logs = await _service.GetByUserIdAsync(userId);
            return Ok(logs);
        }

        // Optional: get logs by chat type
        [HttpGet("type/{chatType}")]
        public async Task<IActionResult> GetByType(string chatType)
        {
            var logs = await _service.GetByChatTypeAsync(chatType);
            return Ok(logs);
        }
    }
}
