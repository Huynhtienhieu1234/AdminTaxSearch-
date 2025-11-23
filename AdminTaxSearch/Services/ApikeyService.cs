using AdminTaxSearch.Data;
using AdminTaxSearch.Models;
using AdminTaxSearch.Models.DTOs;
using Microsoft.EntityFrameworkCore; // <-- thêm dòng này

namespace AdminTaxSearch.Services
{
    public interface IApikeyService
    {
        Task<List<ApikeyDto>> GetByUserAsync(int userId);
        Task<ApikeyDto> CreateAsync(int userId, CreateApikeyDto dto);
        Task<List<ApikeyDto>> GetAllAsync(); // admin
    }

    public class ApikeyService : IApikeyService
    {
        private readonly AppDbContext _db;
        public ApikeyService(AppDbContext db) { _db = db; }

        public async Task<List<ApikeyDto>> GetByUserAsync(int userId)
        {
            return await _db.Apikeys
                .Where(a => a.UserId == userId)
                .Select(a => new ApikeyDto
                {
                    ApikeyId = a.ApikeyId,
                    SystemName = a.SystemName,
                    Apikey = a.Apikey1,
                    Description = a.Description,
                    IsActive = a.IsActive,
                    IssuedDate = a.IssuedDate,
                    UserId = a.UserId
                }).ToListAsync();
        }

        public async Task<List<ApikeyDto>> GetAllAsync()
        {
            return await _db.Apikeys
                .Select(a => new ApikeyDto
                {
                    ApikeyId = a.ApikeyId,
                    SystemName = a.SystemName,
                    Apikey = a.Apikey1,
                    Description = a.Description,
                    IsActive = a.IsActive,
                    IssuedDate = a.IssuedDate,
                    UserId = a.UserId
                }).ToListAsync();
        }

        public async Task<ApikeyDto> CreateAsync(int userId, CreateApikeyDto dto)
        {
            var apikey = new Apikey
            {
                UserId = userId,
                SystemName = dto.SystemName,
                Description = dto.Description,
                Apikey1 = Guid.NewGuid().ToString("N"), // tạo key ngẫu nhiên
                IssuedDate = DateTime.UtcNow,
                IsActive = true
            };

            _db.Apikeys.Add(apikey);
            await _db.SaveChangesAsync();

            return new ApikeyDto
            {
                ApikeyId = apikey.ApikeyId,
                SystemName = apikey.SystemName,
                Apikey = apikey.Apikey1,
                Description = apikey.Description,
                IsActive = apikey.IsActive,
                IssuedDate = apikey.IssuedDate,
                UserId = apikey.UserId
            };
        }
    }
}
