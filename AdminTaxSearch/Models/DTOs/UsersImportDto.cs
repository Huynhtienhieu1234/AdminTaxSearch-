namespace AdminTaxSearch.Models.DTOs
{
    public class UserImportDto
    {
        // Dùng chung cho cả response và request
        public int? UserId { get; set; }          // response có, khi thêm mới thì để null
        public string Username { get; set; } = string.Empty;

        // Khi thêm/sửa thì client gửi Password, server sẽ hash
        public string? Password { get; set; }     // optional, chỉ dùng khi tạo/sửa

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }

        // Chỉ có khi trả dữ liệu ra ngoài
        public DateTime? CreatedDate { get; set; }
    }
}
