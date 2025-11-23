using System.Threading.Tasks;

public interface IAddressService
{
    /// <summary>
    /// Chuyển địa chỉ cũ sang địa chỉ mới dựa trên bảng WardMappings
    /// </summary>
    Task<string?> ConvertAddressAsync(string oldAddress);
}
