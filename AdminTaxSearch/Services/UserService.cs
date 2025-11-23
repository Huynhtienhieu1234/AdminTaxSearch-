using AdminTaxSearch.Data;
using AdminTaxSearch.Models;
using AdminTaxSearch.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AdminTaxSearch.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách
        public async Task<IEnumerable<UserImportDto>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => new UserImportDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    RoleId = u.RoleId,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate
                })
                .ToListAsync();
        }

        // Xem chi tiết
        public async Task<UserImportDto?> GetByIdAsync(int id)
        {
            var u = await _context.Users.FindAsync(id);
            if (u == null) return null;

            return new UserImportDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                IsActive = u.IsActive,
                CreatedDate = u.CreatedDate
            };
        }

        // Thêm mới
        public async Task<UserImportDto> CreateAsync(UserImportDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password ?? string.Empty),
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                RoleId = dto.RoleId ?? 0,
                IsActive = dto.IsActive ?? true,
                CreatedDate = DateTime.UtcNow


            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserImportDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };

        }

        // Sửa
        public async Task<bool> UpdateAsync(int id, UserImportDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Username = dto.Username ?? user.Username;
            user.Email = dto.Email ?? user.Email;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.RoleId = dto.RoleId ?? user.RoleId;
            user.IsActive = dto.IsActive ?? user.IsActive;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _context.SaveChangesAsync();
            return true;
        }

        // Xóa
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
