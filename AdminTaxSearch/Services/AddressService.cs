using AdminTaxSearch.Data;
using AdminTaxSearch.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class AddressService : IAddressService
{
    private readonly AppDbContext _context;

    public AddressService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string?> ConvertAddressAsync(string oldAddress)
    {
        if (string.IsNullOrWhiteSpace(oldAddress))
            return null;

        var mapping = await _context.WardMappings
            .FirstOrDefaultAsync(w => oldAddress.Contains(w.OldWardName) &&
                                      oldAddress.Contains(w.OldDistrictName) &&
                                      oldAddress.Contains(w.OldProvinceName));

        if (mapping == null)
            return null;

        return $"{mapping.NewWardName}, {mapping.NewProvinceName}";
    }
}
