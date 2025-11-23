using AdminTaxSearch.Models.DTOs;
using AdminTaxSearch.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminTaxSearch.Controllers
{
    [ApiController]
    [Route("api/chatbot")]
    public class ChatbotController : ControllerBase
    {
        private readonly ChatbotService _chatbotService;
        private readonly TaxService _taxService;

        public ChatbotController(ChatbotService chatbotService, TaxService taxService)
        {
            _chatbotService = chatbotService;
            _taxService = taxService;
        }

        [HttpPost("ask")]
        public async Task<ChatResponse> Ask([FromBody] ChatRequest request)
        {
            // Nếu message chứa từ khóa thuế, lookup
            if (request.Message.Contains("thuế", StringComparison.OrdinalIgnoreCase))
            {
                var taxInfo = await _taxService.LookupTaxAsync(request.Message);
                return new ChatResponse { Answer = taxInfo };
            }

            // Ngược lại hỏi Ollama
            var answer = await _chatbotService.AskOllamaAsync(request.Message);
            return new ChatResponse { Answer = answer };
        }
    }
}
