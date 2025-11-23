using AdminTaxSearch.Data;
using AdminTaxSearch.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminTaxSearch.Services
{
    public class ChatLogService : IChatLogService
    {
        private readonly AppDbContext _context;

        public ChatLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatLog>> GetAllAsync()
        {
            return await _context.ChatLogs
                                 .Include(c => c.User)
                                 .OrderByDescending(c => c.Timestamp)
                                 .ToListAsync();
        }

        public async Task<ChatLog?> GetByIdAsync(int chatId)
        {
            return await _context.ChatLogs
                                 .Include(c => c.User)
                                 .FirstOrDefaultAsync(c => c.ChatId == chatId);
        }

        public async Task<ChatLog> CreateAsync(ChatLog chatLog)
        {
            chatLog.Timestamp = DateTime.UtcNow;
            _context.ChatLogs.Add(chatLog);
            await _context.SaveChangesAsync();
            return chatLog;
        }

        public async Task<ChatLog?> UpdateAsync(int chatId, ChatLog chatLog)
        {
            var existing = await _context.ChatLogs.FindAsync(chatId);
            if (existing == null) return null;

            existing.Question = chatLog.Question;
            existing.Answer = chatLog.Answer;
            existing.ChatType = chatLog.ChatType;
            existing.ResponseTimeMs = chatLog.ResponseTimeMs;
            existing.IsResolved = chatLog.IsResolved;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int chatId)
        {
            var existing = await _context.ChatLogs.FindAsync(chatId);
            if (existing == null) return false;

            _context.ChatLogs.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ChatLog>> GetByUserIdAsync(int userId)
        {
            return await _context.ChatLogs
                                 .Where(c => c.UserId == userId)
                                 .Include(c => c.User)
                                 .OrderByDescending(c => c.Timestamp)
                                 .ToListAsync();
        }

        public async Task<List<ChatLog>> GetByChatTypeAsync(string chatType)
        {
            return await _context.ChatLogs
                                 .Where(c => c.ChatType == chatType)
                                 .Include(c => c.User)
                                 .OrderByDescending(c => c.Timestamp)
                                 .ToListAsync();
        }
    }
}
