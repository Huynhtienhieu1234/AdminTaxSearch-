using AdminTaxSearch.Data;
using AdminTaxSearch.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;

[ApiController]
[Route("api/[controller]")]
public class AddressController : Controller
 {

        private readonly AppDbContext _context;
          private readonly HttpClient _http;


    public AddressController(AppDbContext context, IHttpClientFactory factory)
        {
            _context = context;
        _http = factory.CreateClient();
    }

        [HttpGet("search-address")]
        public async Task<IActionResult> SearchAddress(
         [FromQuery] string ward,
         [FromQuery] string? district = null,
         [FromQuery] string? province = null)
        {
            if (string.IsNullOrWhiteSpace(ward))
                return BadRequest(new { error = "Vui lòng nhập tên phường/xã" });

            ward = ward.Trim().ToLower();
            district = district?.Trim().ToLower();
            province = province?.Trim().ToLower();

            var query = _context.WardMappings.AsQueryable();

            // Lọc theo ward (bắt buộc)
            query = query.Where(x =>
                x.OldWardName!.ToLower().Contains(ward) ||
                x.NewWardName!.ToLower().Contains(ward));

            // Lọc thêm theo district nếu có
            if (!string.IsNullOrWhiteSpace(district))
                query = query.Where(x =>
                    x.OldDistrictName!.ToLower().Contains(district));

            // Lọc thêm theo province nếu có
            if (!string.IsNullOrWhiteSpace(province))
                query = query.Where(x =>
                    x.OldProvinceName!.ToLower().Contains(province));

            var results = await query
                .Select(x => new
                {
                    old = new
                    {
                        ward = x.OldWardName,
                        district = x.OldDistrictName,
                        province = x.OldProvinceName
                    },
                    updated = new
                    {
                        ward = x.NewWardName,
                        province = x.NewProvinceName
                    }
                })
                .ToListAsync();

            return Ok(results);
        }


        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _context.WardMappings
                .Select(x => x.OldProvinceName)
                .Where(x => x != null)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            return Ok(provinces);
        }

    [HttpGet("districts")]
    public async Task<IActionResult> GetDistricts([FromQuery] string province)
    {
        if (string.IsNullOrWhiteSpace(province))
            return BadRequest(new { error = "Thiếu tỉnh/thành phố" });

        province = province.Trim().ToLower();

        var districts = await _context.WardMappings
            .Where(x => x.OldProvinceName!.ToLower().Trim() == province)
            .Select(x => x.OldDistrictName)
            .Where(x => x != null)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        return Ok(districts);
    }

    [HttpGet("wards")]
    public async Task<IActionResult> GetWards([FromQuery] string district, [FromQuery] string province)
    {
        if (string.IsNullOrWhiteSpace(district) || string.IsNullOrWhiteSpace(province))
            return BadRequest(new { error = "Thiếu quận/huyện hoặc tỉnh" });

        district = district.Trim().ToLower();
        province = province.Trim().ToLower();

        var wards = await _context.WardMappings
            .Where(x => x.OldDistrictName!.ToLower().Trim() == district &&
                        x.OldProvinceName!.ToLower().Trim() == province)
            .Select(x => x.OldWardName)
            .Where(x => x != null)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        return Ok(wards);
    }

    // Endpoint tra từ tọa độ => old address (reverse geocode)
    [HttpGet("reverse-geocode")]
    public async Task<IActionResult> ReverseGeocode(double lat, double lng)
    {
        // Ví dụ dùng BigDataCloud API miễn phí:
        var apiUrl = $"https://api.bigdatacloud.net/data/reverse-geocode-client?latitude={lat}&longitude={lng}&localityLanguage=vi";
        var resp = await _http.GetAsync(apiUrl);
        if (!resp.IsSuccessStatusCode) return BadRequest("Không reverse được tọa độ");

        var json = await resp.Content.ReadFromJsonAsync<BigDataCloudResult>();

        // json sẽ chứa city, state, country... tùy API
        // Giờ lấy name tỉnh / quận / phường (tuỳ API) để tìm mapping cũ

        string province = json.principalSubdivision; // ví dụ
        string city = json.city; // hoặc model API trả

        // Tìm mapping WardMapping theo tỉnh + quận + (có thể phường nếu map được)
        var mapping = await _context.WardMappings
            .Where(x => x.OldProvinceName.ToLower().Contains(province.ToLower()))
            // ... lọc thêm theo district / ward nếu muốn
            .FirstOrDefaultAsync();

        if (mapping == null)
            return NotFound(new { error = "Không tìm thấy mapping hành chính." });

        return Ok(mapping);
    }

    // Endpoint convert cũ → mới (giống như bạn đã có)
    [HttpGet("convert")]
    public async Task<IActionResult> ConvertAddress([FromQuery] string oldAddress)
    {
        if (string.IsNullOrWhiteSpace(oldAddress))
            return BadRequest(new { error = "Vui lòng nhập địa chỉ cũ" });

        oldAddress = oldAddress.Trim();

        // Tách oldAddress thành ward, district, province
        var parts = oldAddress.Split(',', StringSplitOptions.TrimEntries);
        string? ward = parts.Length > 0 ? parts[0] : null;
        string? district = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]) ? parts[1] : null;
        string? province = parts.Length > 2 ? parts[2] : null;

        var query = _context.WardMappings.AsQueryable();

        if (!string.IsNullOrEmpty(ward))
            query = query.Where(x => x.OldWardName!.ToLower().Contains(ward.ToLower()));

        if (!string.IsNullOrEmpty(district))
            query = query.Where(x => x.OldDistrictName!.ToLower().Contains(district.ToLower()));

        if (!string.IsNullOrEmpty(province))
            query = query.Where(x => x.OldProvinceName!.ToLower().Contains(province.ToLower()));

        var result = await query.FirstOrDefaultAsync();

        if (result == null)
            return NotFound(new { error = "Không tìm thấy địa chỉ tương ứng" });

        return Ok(result);
    }

}

// Model kết quả từ BigDataCloud
public class BigDataCloudResult
{
    public string? city { get; set; }
    public string? principalSubdivision { get; set; }
    public string? countryName { get; set; }
}