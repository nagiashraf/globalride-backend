using GlobalRide.Domain.Cars;
using GlobalRide.Domain.Rentals;

using Microsoft.EntityFrameworkCore;

namespace GlobalRide.Infrastructure.Persistence.Repositories;

internal sealed class CarRepository(AppDbContext context) : ICarRepository
{
    public async Task<int> CountByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await context.Set<Car>()
            .CountAsync(c => c.BranchId == branchId, cancellationToken);
    }

    public async Task<int> CountIncomingByBranchIdAsync(
        Guid branchId,
        DateTime incomingDate,
        CancellationToken cancellationToken = default)
    {
        return await context.Set<Rental>()
            .CountAsync(
                r =>
                    r.PickupBranchId != branchId &&
                    r.DropoffBranchId == branchId &&
                    r.Period.Dropoff >= incomingDate.Date &&
                    r.Period.Dropoff < incomingDate.Date.AddDays(1) &&
                    r.Status != RentalStatus.Canceled,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Car>> ListAvailableByBranchAndDatesAsync(
        Guid pickupBranchId,
        DateTime pickupDate,
        DateTime dropoffDate,
        CancellationToken cancellationToken = default)
    {
        FormattableString sql = $@"
            SELECT C.Id, C.BranchId, C.CarTypeId, C.Make, C.Model, C.Year,
                C.SeatsCount, C.DailyRate, C.TransmissionType, C.FuelType
            FROM dbo.Cars AS C
            WHERE C.BranchId = {pickupBranchId}
            AND NOT EXISTS (
                SELECT * FROM dbo.Rentals AS R
                WHERE R.CarId = C.Id
                AND R.Period_Pickup <= {dropoffDate}
                AND R.Period_Dropoff >= {pickupDate}
            )";

        var cars = await context.Set<Car>()
            .FromSql(sql)
            .Include(c => c.CarType)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return cars;
    }
}
