using AdminTaxSearch.Models;

public class SearchHistory
{
    public int Id { get; set; }  // PK

    // Người dùng nhập vào (MST, CCCD, địa chỉ...)
    public string? InputText { get; set; }

    // Kết quả trả về (tên doanh nghiệp, địa chỉ, thông tin cá nhân...)
    public string? ResultText { get; set; }

    // Thời điểm tra cứu
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Nếu có đăng nhập
    public int? UserId { get; set; }
    public virtual User? User { get; set; }

    // Nếu muốn log IP
    public string? IpAddress { get; set; }
    public DateTime? Timestamp { get; set; }


}
