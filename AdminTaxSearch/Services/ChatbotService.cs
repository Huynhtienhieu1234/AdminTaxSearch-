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
                    model = "llama3:latest",  // ← ĐỔI THÀNH "llama3:latest" hoặc "llama3:8b"
                    prompt = prompt,
                    stream = false,
                    system = "Bạn là trợ lý AI tra cứu thuế thông minh của Việt Nam. " +
                             "Luôn luôn trả lời bằng tiếng Việt có dấu. " +
                             "Cung cấp thông tin chính xác, chi tiết về mã số thuế, CCCD, thông tin doanh nghiệp và cá nhân. " +
                             "Hãy thân thiện, chuyên nghiệp và dễ hiểu. " +
                             "QUAN TRỌNG: Tất cả câu trả lời phải bằng tiếng Việt.",

                    options = new
                    {
                        temperature = 0.7,
                        num_predict = 500
                    }
                };

                // Đổi endpoint thành API chuẩn của Ollama
                var response = await _http.PostAsJsonAsync(
                    "http://localhost:11434/api/generate",
                    request
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Ollama Error: {errorBody}");
                    return $"Ollama server trả về lỗi: {response.StatusCode}";
                }

                using var jsonDoc = await JsonDocument.ParseAsync(
                    await response.Content.ReadAsStreamAsync()
                );
                var root = jsonDoc.RootElement;

                string aiReply = null;

                // Ollama API chuẩn trả về "response"
                if (root.TryGetProperty("response", out var responseProp))
                {
                    aiReply = responseProp.GetString()?.Trim();
                }

                // Fallback nếu rỗng
                if (string.IsNullOrWhiteSpace(aiReply))
                    aiReply = "Chatbot chưa hiểu, bạn nói lại được không?";

                return aiReply;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception: {ex.Message}");
                return $"Không thể kết nối tới Ollama (Lỗi: {ex.Message})";
            }
        }
    }
}