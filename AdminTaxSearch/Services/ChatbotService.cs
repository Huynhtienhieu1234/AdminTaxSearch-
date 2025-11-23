using System.Text.Json;
using System.Text;

namespace AdminTaxSearch.Services
{
    public class ChatbotService
    {
        private readonly HttpClient _http;

        public ChatbotService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> AskOllamaAsync(string prompt)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(prompt))
                    return "Vui lòng nhập nội dung.";

                // --- Chuẩn bị payload Ollama ---
                var request = new
                {
                    model = "llama3",       // tên model chính xác
                    prompt = prompt,        // prompt người dùng
                    temperature = 0.7,      // tạo câu trả lời tự nhiên
                    max_tokens = 300
                };

                var response = await _http.PostAsJsonAsync("http://localhost:11434/v1/completions", request);

                if (!response.IsSuccessStatusCode)
                    return $"Ollama server trả về lỗi: {response.StatusCode}";

                using var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
                var root = jsonDoc.RootElement;

                string aiReply = null;

                // Ollama trả về text trong choices[0].text
                if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var firstChoice = choices[0];
                    if (firstChoice.TryGetProperty("text", out var textProp))
                    {
                        aiReply = textProp.GetString()?.Trim();
                    }
                }

                // Fallback nếu rỗng
                if (string.IsNullOrWhiteSpace(aiReply))
                    aiReply = "Chatbot chưa hiểu, bạn nói lại được không?";

                return aiReply;
            }
            catch (Exception ex)
            {
                return $"Không thể kết nối tới Ollama (Lỗi: {ex.Message})";
            }
        }

    }
}
