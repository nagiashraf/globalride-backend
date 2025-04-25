using GlobalRide.Domain.Branches;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlobalRide.Infrastructure.Persistence.Configurations;

internal sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");

        builder.Property(b => b.CountryCode)
            .HasMaxLength(2);

        builder.Property(b => b.TimeZone)
            .HasMaxLength(100);

        builder.Property(b => b.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.OwnsOne(b => b.Coordinate);

        builder.OwnsMany(
            b => b.Translations, t =>
            {
                t.ToTable("BranchTranslations");

                t.WithOwner().HasForeignKey("BranchId");

                t.HasKey("BranchId", "LanguageCode");

                t.Property(t => t.LanguageCode)
                    .HasConversion<string>()
                    .HasMaxLength(5);

                t.Property(t => t.Name)
                    .HasMaxLength(100);

                t.Property(t => t.City)
                    .HasMaxLength(100);

                t.Property(t => t.Country)
                    .HasMaxLength(100);
            });
    }
}
