using GlobalRide.Domain.CarTypes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlobalRide.Infrastructure.Persistence.Configurations;

internal sealed class CarTypeConfiguration : IEntityTypeConfiguration<CarType>
{
    public void Configure(EntityTypeBuilder<CarType> builder)
    {
        builder.ToTable("CarTypes");

        builder.Property(c => c.Category)
            .HasMaxLength(50);

        builder.Property(c => c.OneWayFeeMultiplier)
            .HasPrecision(18, 2);
    }
}
