using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Cars;
using GlobalRide.Domain.Rentals;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlobalRide.Infrastructure.Persistence.Configurations;

internal sealed class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.ToTable("Rentals");

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.TotalCost)
            .HasPrecision(18, 2);

        builder.OwnsOne(r => r.Period);

        builder.HasOne<Car>()
            .WithMany()
            .HasForeignKey(r => r.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(r => r.PickupBranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(r => r.DropoffBranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
