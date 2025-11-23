using AdminTaxSearch.Services;
using System.Text.Json;

public class MasothueService : IMasothueService
{
    private readonly HttpClient _http;

    public MasothueService(HttpClient http)
    {
        _http = http;
    }

    public async Task<Dictionary<string, string>?> GetPersonalInfoAsync(string cccd)
    {
        // Thay URL này bằng API thực tế hoặc local Playwright server
        var url = $"https://localhost:7215/api/masothue?cccd={cccd}";
        try
        {
            var resp = await _http.GetFromJsonAsync<Dictionary<string, object>>(url);
            if (resp == null || !resp.ContainsKey("data"))
                return null;

            var data = resp["data"] as JsonElement?;
            if (data == null)
                return null;

            var dict = new Dictionary<string, string>();
            foreach (var prop in data.Value.EnumerateObject())
            {
                dict[prop.Name] = prop.Value.GetString() ?? "";
            }
            dict["input"] = cccd;
            dict["detail_url"] = resp.ContainsKey("detail_url") ? resp["detail_url"]?.ToString() ?? "" : "";
            return dict;
        }
        catch
        {
            return null;
        }
    }
}