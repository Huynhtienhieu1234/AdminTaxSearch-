
using AdminTaxSearch.Models.DTOs;

namespace AdminTaxSearch.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(AuthRequestDto dto);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<List<UserDto>> GetAllUsersAsync();
    }
}
