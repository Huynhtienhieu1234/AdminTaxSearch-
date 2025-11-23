using AdminTaxSearch.Data;
using AdminTaxSearch.Models;
using AdminTaxSearch.Models.DTOs;
using AdminTaxSearch.Options;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace AdminTaxSearch.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly JwtOptions _jwtOptions;

        public AuthService(AppDbContext db, IOptions<JwtOptions> jwtOptions)
        {
            _db = db;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Kiểm tra định dạng email Gmail
            if (!Regex.IsMatch(dto.Email ?? "", @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
                throw new Exception("Email phải là địa chỉ Gmail hợp lệ.");

            // Kiểm tra username đã tồn tại
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Tên người dùng đã tồn tại.");

            // Kiểm tra email đã tồn tại
            if (await _db.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
                throw new Exception("Email đã được sử dụng.");

            // Tạo user mới
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = 2,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Gán Role cho user để tránh lỗi null khi tạo token
            user.Role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleId == user.RoleId);

            var token = GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role.RoleName,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes)
            };
        }



        public async Task<AuthResponseDto?> LoginAsync(AuthRequestDto dto)
        {
            var user = await _db.Users
                .Include(u => u.Role) // <-- thêm dòng này
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

            var token = GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role.RoleName,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes)
            };
        }


        private string GenerateToken(User user)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim("uid", user.UserId.ToString()),
        new Claim(ClaimTypes.Role, user.Role.RoleName) // <-- thêm dòng này
    };

    var token = new JwtSecurityToken(
        issuer: _jwtOptions.Issuer,
        audience: _jwtOptions.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}


        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new Exception("Current password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _db.SaveChangesAsync();
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _db.Users
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    IsActive = u.IsActive
                }).ToListAsync();
        }

    }
}
