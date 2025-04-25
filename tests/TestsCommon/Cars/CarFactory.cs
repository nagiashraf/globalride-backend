using System.Text;

using GlobalRide.Domain.Cars;
using GlobalRide.Domain.CarTypes;

namespace TestsCommon.Cars;

public static class CarFactory
{
    public static Car Create(
        string? vin = null,
        Guid? branchId = null,
        Guid? carTypeId = null,
        string make = "Honda",
        string model = "Accord",
        int year = 2025,
        int seatsCount = 5,
        decimal dailyRate = 65.99m,
        TransmissionType transmissionType = TransmissionType.Automatic,
        FuelType fuelType = FuelType.Gasoline,
        CarType? carType = null)
        => new(
                vin ?? GenerateRandomVin(),
                branchId ?? Guid.NewGuid(),
                carTypeId ?? Guid.NewGuid(),
                make,
                model,
                year,
                seatsCount,
                dailyRate,
                transmissionType,
                fuelType,
                carType);

    public static string GenerateRandomVin()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        int vinLength = 17;
        var stringBuilder = new StringBuilder(vinLength);

        for (int i = 0; i < vinLength; i++)
        {
            int index = random.Next(chars.Length);
            stringBuilder.Append(chars[index]);
        }

        return stringBuilder.ToString();
    }
}
