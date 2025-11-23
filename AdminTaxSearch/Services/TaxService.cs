using AdminTaxSearch.Data;
using Microsoft.EntityFrameworkCore; // bắt buộc để dùng FirstOrDefaultAsync

namespace AdminTaxSearch.Services
{
    public class TaxService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _http;

        public TaxService(AppDbContext context, HttpClient http)
        {
            _context = context;
            _http = http;
        }

        public async Task<string> LookupTaxAsync(string taxIdOrName)
        {
            // 1. Tìm trong database cá nhân
            var person = await _context.PersonalInfos.FirstOrDefaultAsync(p => p.TaxId == taxIdOrName || p.FullName.Contains(taxIdOrName));
            if (person != null)
                return $"Cá nhân: {person.FullName}, Mã số thuế: {person.TaxId}";

            // 2. Tìm trong database công ty
            var company = await _context.BusinessInfos.FirstOrDefaultAsync(c => c.TaxId == taxIdOrName || c.BusinessName.Contains(taxIdOrName));
            if (company != null)
                return $"Doanh nghiệp: {company.BusinessName}, Mã số thuế: {company.TaxId}";

            // 3. Nếu không có, gọi API Masothue
            var response = await _http.GetFromJsonAsync<dynamic>($"https://masothue.vn/api/lookup?query={taxIdOrName}");
            if (response != null)
                return $"Tra cứu Masothue: {response}";

            return "Không tìm thấy thông tin thuế";
        }
    }
}
