using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Cars;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlobalRide.Infrastructure.Persistence.Configurations;

internal sealed class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");

        builder.Property(c => c.Id)
            .HasMaxLength(17)
            .IsFixedLength();

        builder.Property(c => c.Make)
            .HasMaxLength(50);

        builder.Property(c => c.Model)
            .HasMaxLength(50);

        builder.Property(c => c.DailyRate)
            .HasPrecision(18, 2);

        builder.Property(c => c.TransmissionType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.FuelType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(c => c.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CarType)
            .WithMany()
            .HasForeignKey(c => c.CarTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
