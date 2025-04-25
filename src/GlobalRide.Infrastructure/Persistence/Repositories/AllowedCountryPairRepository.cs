using GlobalRide.Domain.AllowedCountryPairs;

using Microsoft.EntityFrameworkCore;

namespace GlobalRide.Infrastructure.Persistence.Repositories;

internal sealed class AllowedCountryPairRepository(AppDbContext context) : IAllowedCountryPairRepository
{
    public async Task<bool> ExistsAsync(string pickupCountryCode, string dropoffCountryCode, CancellationToken cancellationToken = default)
    {
        return await context.Set<AllowedCountryPair>()
            .AnyAsync(cp => cp.CountryCodeFrom == pickupCountryCode && cp.CountryCodeTo == dropoffCountryCode, cancellationToken);
    }
}
