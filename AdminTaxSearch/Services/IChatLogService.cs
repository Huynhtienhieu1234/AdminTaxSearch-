using AdminTaxSearch.Models;

namespace AdminTaxSearch.Services
{
    public interface IChatLogService
    {
        Task<List<ChatLog>> GetAllAsync();
        Task<ChatLog?> GetByIdAsync(int chatId);
        Task<ChatLog> CreateAsync(ChatLog chatLog);
        Task<ChatLog?> UpdateAsync(int chatId, ChatLog chatLog);
        Task<bool> DeleteAsync(int chatId);

        Task<List<ChatLog>> GetByUserIdAsync(int userId);
        Task<List<ChatLog>> GetByChatTypeAsync(string chatType);
    }
}
