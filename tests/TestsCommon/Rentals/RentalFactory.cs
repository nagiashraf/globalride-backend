using GlobalRide.Domain.Rentals;

using TestsCommon.Cars;

namespace TestsCommon.Rentals;

public static class RentalFactory
{
    public static Rental Create(
        Guid? id = null,
        string? carId = null,
        Guid? pickupBranchId = null,
        Guid? dropoffBranchId = null,
        DateTime? pickupDate = null,
        DateTime? dropoffDate = null,
        decimal totalCost = 100m,
        RentalStatus status = RentalStatus.Confirmed,
        DateTime? createdAt = null)
        => new(
            id ?? Guid.NewGuid(),
            carId ?? CarFactory.GenerateRandomVin(),
            pickupBranchId ?? Guid.NewGuid(),
            dropoffBranchId ?? Guid.NewGuid(),
            period: new RentalPeriod(
                pickupDate ?? new DateTime(2025, 1, 1, 0, 0, 0),
                dropoffDate ?? new DateTime(2025, 1, 2, 0, 0, 0)),
            totalCost,
            status,
            createdAt ?? DateTime.UtcNow
        );
}
