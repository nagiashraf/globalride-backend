using GlobalRide.Domain.Branches;

using Microsoft.EntityFrameworkCore;

namespace GlobalRide.Infrastructure.Persistence.Repositories;

internal sealed class BranchRepository(AppDbContext context) : IBranchRepository
{
    #pragma warning disable CA2016

    public async Task<Branch?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Set<Branch>().FindAsync(id, cancellationToken);

#pragma warning restore CA2016

    public async Task<IReadOnlyList<BranchSearchResultResponse>> ListTranslationsAsync(CancellationToken cancellationToken = default)
    {
        var results = await context.Set<Branch>()
            .AsNoTracking()
            .SelectMany(
                branch => branch.Translations,
                (branch, translation) => new BranchSearchResultResponse(
                    branch.Id,
                    translation.LanguageCode,
                    translation.Name,
                    translation.City,
                    translation.Country,
                    branch.Type,
                    branch.TimeZone))
            .ToListAsync(cancellationToken);

        return results;
    }
}
