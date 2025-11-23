using AdminTaxSearch.Models.DTOs;
using AdminTaxSearch.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminTaxSearch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // Đăng ký tài khoản
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Nếu lỗi liên quan đến email hoặc username trùng
                if (ex.Message.Contains("Email Gmail đã được sử dụng") ||
                    ex.Message.Contains("Tên người dùng đã tồn tại"))
                {
                    return Conflict(new { error = ex.Message }); // 409 Conflict
                }

                // Các lỗi khác
                return BadRequest(new { error = ex.Message }); // 400 BadRequest
            }
        }

        // Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(new { error = "Tên đăng nhập hoặc mật khẩu không đúng" });

            return Ok(result);
        }

        // Đổi mật khẩu
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            try
            {
                await _authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Lấy danh sách tất cả người dùng (chỉ admin)
        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}
