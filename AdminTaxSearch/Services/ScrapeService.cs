using AdminTaxSearch.Data;
using System.Xml;
using AdminTaxSearch.Models;
using Newtonsoft.Json;

namespace AdminTaxSearch.Services
{
    public class ScrapeService
    {
        private readonly AppDbContext _db;

        public ScrapeService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ScrapeResult> SaveSearchResultAsync(int userId, string cccd, object apiResult)
        {
            // Chuyển kết quả object thành JSON
            string rawJson = JsonConvert.SerializeObject(apiResult, Newtonsoft.Json.Formatting.Indented);

            var scrape = new ScrapeResult
            {
                //UserId = userId,
                CreatedAt = DateTime.Now
            };

            _db.ScrapeResults.Add(scrape);
            await _db.SaveChangesAsync();

            return scrape;
        }

        // Nếu muốn liên kết PersonalInfo
        public async Task LinkPersonalInfoAsync(int personId, int scrapeId)
        {
            var person = await _db.PersonalInfos.FindAsync(personId);
            if (person != null)
            {
                person.ScrapeResultId = scrapeId;
                await _db.SaveChangesAsync();
            }
        }
    }
}