using GlobalRide.Domain.AllowedCountryPairs;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlobalRide.Infrastructure.Persistence.Configurations;

internal sealed class AllowedCountryPairConfiguration : IEntityTypeConfiguration<AllowedCountryPair>
{
    public void Configure(EntityTypeBuilder<AllowedCountryPair> builder)
    {
        builder.ToTable("CountryPairs");

        builder.HasKey(cp => new { cp.CountryCodeFrom, cp.CountryCodeTo });

        builder.Property(cp => cp.CountryCodeFrom)
            .HasMaxLength(2);

        builder.Property(cp => cp.CountryCodeTo)
            .HasMaxLength(2);
    }
}
