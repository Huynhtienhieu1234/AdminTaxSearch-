using AdminTaxSearch.Data;
using AdminTaxSearch.Models;
using AdminTaxSearch.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly AppDbContext _context;

    public SearchController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Cccd))
            return BadRequest(new { error = "CCCD/TaxID không hợp lệ" });

        try
        {
            string cccd = req.Cccd.Trim();


            var personal = await _context.PersonalInfos
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.TaxId == cccd || p.Idnumber == cccd);

            if (personal != null)
            {
                return Ok(new SearchResponse
                {
                    Input = cccd,
                    Found = true,
                    Data = new TaxInfo
                    {
                        TaxId = personal.TaxId ?? "",
                        Idnumber = personal.Idnumber,
                        Name = personal.FullName,
                        Address = personal.Address,
                        Status = personal.Occupation,
                        DateOfBirth = personal.DateOfBirth,
                        Type = "Personal"
                    },
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Source = "db"  // Dữ liệu từ DB
                });
            }

            // 2. Kiểm tra BusinessInfo
            var business = await _context.BusinessInfos
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.TaxId == cccd);

            if (business != null)
            {
                return Ok(new SearchResponse
                {
                    Input = cccd,
                    Found = true,
                    Data = new TaxInfo
                    {
                        TaxId = business.TaxId,
                        Name = business.BusinessName,
                        Address = business.Address,
                        Status = business.Status,
                        FoundingDate = business.FoundingDate,
                        Representative = business.Representative,
                        Type = "Business"
                    },
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Source = "db"  // Dữ liệu từ DB
                });
            }

            // 3. Nếu DB không có, scrape mới
            var result = await ScrapeAndSaveTaxInfoAsync(cccd);
            return Ok(new SearchResponse
            {
                Input = cccd,
                Found = true,
                Data = result,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Source = "scrape" // Dữ liệu từ web scrape
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR: {ex}");
            return StatusCode(500, new { error = ex.Message });
        }
    }


    public class SearchRequest
    {
        public string Cccd { get; set; } = string.Empty;
    }

    private async Task<TaxInfo> ScrapeAndSaveTaxInfoAsync(string taxIdOrCccd)
    {
        var scrapeData = await RunScrapeAsync(taxIdOrCccd);

        if (string.IsNullOrEmpty(scrapeData.TaxId))
            throw new Exception("Không tìm thấy TaxId từ kết quả scrape");

        var scrapeResult = new ScrapeResult
        {
            RawJson = JsonSerializer.Serialize(scrapeData),
            CreatedAt = DateTime.Now
        };
        _context.ScrapeResults.Add(scrapeResult);
        await _context.SaveChangesAsync();

        bool isPersonal;

        if (scrapeData.TaxId.Contains("-"))
        {
            isPersonal = false;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(scrapeData.Type))
                isPersonal = scrapeData.Type.Contains("Cá nhân");
            else
                isPersonal = true; 
        }
        TaxInfo dto = new TaxInfo
        {
            TaxId = scrapeData.TaxId,
            Name = scrapeData.Name,
            Type = isPersonal ? "Personal" : "Business",
            Representative = scrapeData.Representative,
            Address = scrapeData.Address,
            Status = scrapeData.Status,
            DateOfBirth = isPersonal ? scrapeData.DateOfBirth : null,
            FoundingDate = !isPersonal ? scrapeData.FoundingDate : null
        };

        int? personalId = null;
        int? businessId = null;

        if (isPersonal)
        {
            var existing = await _context.PersonalInfos
                .FirstOrDefaultAsync(p => p.TaxId == scrapeData.TaxId);

            if (existing != null)
            {
                existing.FullName = dto.Name;
                existing.Address = dto.Address;
                existing.Occupation = dto.Status;
                existing.DateOfBirth = dto.DateOfBirth;
                existing.UpdateDate = DateTime.Now;
                existing.ScrapeResultId = scrapeResult.Id;

                // Chỉ lưu CCCD nếu là 12 số
                if (!string.IsNullOrWhiteSpace(taxIdOrCccd) && taxIdOrCccd.Length == 12)
                {
                    existing.Idnumber = taxIdOrCccd;
                }

                _context.PersonalInfos.Update(existing);
                personalId = existing.PersonId;
            }
            else
            {
                var personal = new PersonalInfo
                {
                    FullName = dto.Name,
                    TaxId = dto.TaxId,
                    Address = dto.Address,
                    Occupation = dto.Status,
                    DateOfBirth = dto.DateOfBirth,
                    UpdateDate = DateTime.Now,
                    ScrapeResultId = scrapeResult.Id,
                    // Chỉ lưu CCCD nếu là 12 số
                    Idnumber = (!string.IsNullOrWhiteSpace(taxIdOrCccd) && taxIdOrCccd.Length == 12)
                                ? taxIdOrCccd
                                : null
                };
                _context.PersonalInfos.Add(personal);
                await _context.SaveChangesAsync();
                personalId = personal.PersonId;
            }
        }

        else
        {
            var existing = await _context.BusinessInfos
                .FirstOrDefaultAsync(b => b.TaxId == scrapeData.TaxId);
            if (existing != null)
            {
                existing.BusinessName = dto.Name;
                existing.Representative = dto.Representative;
                existing.Address = dto.Address;
                existing.Status = dto.Status;
                existing.FoundingDate = dto.FoundingDate;
                _context.BusinessInfos.Update(existing);
                businessId = existing.BusinessId;
            }
            else
            {
                var business = new BusinessInfo
                {
                    TaxId = dto.TaxId,
                    BusinessName = dto.Name,
                    Representative = dto.Representative,
                    Address = dto.Address,
                    Status = dto.Status,
                    FoundingDate = dto.FoundingDate,
                };
                _context.BusinessInfos.Add(business);
                await _context.SaveChangesAsync();
                businessId = business.BusinessId;
            }
        }

        // Lưu SearchHistory mới
        _context.SearchHistories.Add(new SearchHistory
        {
            InputText = taxIdOrCccd,
            ResultText = JsonSerializer.Serialize(dto),
            UserId = null, // nếu chưa có user login, hoặc lấy từ context
            IpAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString(),
            CreatedAt = DateTime.Now
        });

        await _context.SaveChangesAsync();

        return dto;
    }

    private async Task<(string TaxId, string Name, string? Representative, string? Address,
                         string? Status, DateOnly? DateOfBirth, DateOnly? FoundingDate,
                         bool IsPersonal, string? Type)>
    RunScrapeAsync(string taxIdOrCccd)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 900 },
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0",
            Locale = "vi-VN"
        });

        var page = await context.NewPageAsync();

        await page.GotoAsync("https://masothue.com/tra-cuu-ma-so-thue-ca-nhan/",
                             new PageGotoOptions { Timeout = 60000, WaitUntil = WaitUntilState.DOMContentLoaded });

        var input = await page.WaitForSelectorAsync("#search, input[name='q'], input.search-field, input.tax-search-input",
                                                   new PageWaitForSelectorOptions { Timeout = 10000 });
        if (input == null) throw new Exception("Không tìm thấy ô nhập dữ liệu");
        await input.FillAsync(taxIdOrCccd);

        var btn = await page.QuerySelectorAsync("button[type='submit'], button.btn-search-submit, button.btn-search");
        if (btn != null)
        {
            await btn.ClickAsync();
            await page.WaitForTimeoutAsync(500);
        }
        else
        {
            await page.Keyboard.PressAsync("Enter");
            await page.WaitForTimeoutAsync(500);
        }

        var table = await page.WaitForSelectorAsync("table.table-taxinfo",
                                                    new PageWaitForSelectorOptions { Timeout = 15000 });
        if (table == null) throw new Exception("Không tìm thấy bảng kết quả");

        string? name = await TryGetText(page, "table.table-taxinfo th span.copy");
        string? taxId = await TryGetText(page, "table.table-taxinfo td[itemprop='taxID'] span.copy");
        string? status = await TryGetText(page, "#tax-status-html");
        string? representative = null;
        string? startDate = null;
        string? type = null;

        var rows = await table.QuerySelectorAllAsync("tr");
        foreach (var tr in rows)
        {
            var tds = await tr.QuerySelectorAllAsync("td");
            if (tds.Count < 2) continue;

            string label = (await tds[0].InnerTextAsync()).Trim();
            string val = (await tds[1].InnerTextAsync()).Trim();

            if (label.Contains("Người đại diện") || label.Contains("Người nộp"))
                representative = val;

            if (label.Contains("Ngày hoạt động") || label.Contains("Ngày cấp"))
                startDate = val;

            if (label.Contains("Loại hình") || label.Contains("Type"))
                type = val;
        }

        string? address = await TryGetMetaContent(page, "meta[name='description']");

        bool isPersonal = !string.IsNullOrWhiteSpace(type) && type.Contains("Cá nhân");
        DateOnly? dob = null;
        DateOnly? founding = null;

        if (!string.IsNullOrWhiteSpace(startDate) && DateTime.TryParse(startDate, out var dtParsed))
        {
            if (isPersonal)
                dob = DateOnly.FromDateTime(dtParsed);
            else
                founding = DateOnly.FromDateTime(dtParsed);
        }

        return (
            TaxId: taxId ?? taxIdOrCccd,
            Name: name ?? "",
            Representative: representative,
            Address: address,
            Status: status,
            DateOfBirth: dob,
            FoundingDate: founding,
            IsPersonal: isPersonal,
            Type: type
        );
    }

    private async Task<string?> TryGetText(IPage page, string selector)
    {
        try
        {
            var el = await page.QuerySelectorAsync(selector);
            return el == null ? null : (await el.InnerTextAsync()).Trim();
        }
        catch { return null; }
    }

    private async Task<string?> TryGetMetaContent(IPage page, string selector)
    {
        try
        {
            var meta = await page.QuerySelectorAsync(selector);
            if (meta == null) return null;
            var content = await meta.GetAttributeAsync("content");
            if (string.IsNullOrWhiteSpace(content)) return null;
            var parts = content.Split(" - ");
            return parts.Length > 1 ? parts[^1].Trim() : null;
        }
        catch { return null; }
    }

    [HttpPut("update-address")]
    public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.TaxId) || string.IsNullOrWhiteSpace(req.Address))
            return BadRequest(new { error = "TaxId và Address không hợp lệ" });

        // Tìm thông tin cá nhân
        var personal = await _context.PersonalInfos
            .FirstOrDefaultAsync(p => p.TaxId == req.TaxId);

        if (personal != null)
        {
            personal.Address = req.Address;
            personal.UpdateDate = DateTime.Now;
            _context.PersonalInfos.Update(personal);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, type = "personal" });
        }

        // Nếu không phải cá nhân, tìm doanh nghiệp
        var business = await _context.BusinessInfos
            .FirstOrDefaultAsync(b => b.TaxId == req.TaxId);

        if (business != null)
        {
            business.Address = req.Address;
            _context.BusinessInfos.Update(business);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, type = "business" });
        }

        return NotFound(new { error = "Không tìm thấy thông tin với TaxId này" });
    }

    public class UpdateAddressRequest
    {
        public string TaxId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }



}
