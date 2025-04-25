using GlobalRide.Domain.CarTypes;
using GlobalRide.Domain.Common;
using GlobalRide.Domain.Rentals;

namespace GlobalRide.Domain.Cars;

/// <summary>
/// Represents a car entity in the domain.
/// </summary>
public sealed class Car : Entity<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Car"/> class with the specified properties.
    /// </summary>
    /// <param name="id">The Vehicle Identification Number (VIN) of the car, which serves as its unique identifier.</param>
    /// <param name="branchId">The unique identifier of the branch where the car is located.</param>
    /// <param name="carTypeId">The unique identifier of the car type associated with this car.</param>
    /// <param name="make">The make (manufacturer) of the car.</param>
    /// <param name="model">The model of the car.</param>
    /// <param name="year">The manufacturing year of the car.</param>
    /// <param name="seatsCount">The number of seats in the car.</param>
    /// <param name="dailyRate">The daily rental rate for the car.</param>
    /// <param name="transmissionType">The transmission type of the car, as defined by <see cref="TransmissionType"/>.</param>
    /// <param name="fuelType">The fuel type of the car, as defined by <see cref="FuelType"/>.</param>
    /// <param name="carType">The car type entity associated with this car.</param>
    public Car(
        string id,
        Guid branchId,
        Guid carTypeId,
        string make,
        string model,
        int year,
        int seatsCount,
        decimal dailyRate,
        TransmissionType transmissionType,
        FuelType fuelType,
        CarType? carType)
        : base(id)
    {
        BranchId = branchId;
        CarTypeId = carTypeId;
        Make = make;
        Model = model;
        Year = year;
        SeatsCount = seatsCount;
        DailyRate = dailyRate;
        TransmissionType = transmissionType;
        FuelType = fuelType;
        CarType = carType;
    }

    private Car()
    {
    }

    /// <summary>
    /// Gets the unique identifier of the branch where the car is located.
    /// </summary>
    public Guid BranchId { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the car type associated with this car.
    /// </summary>
    public Guid CarTypeId { get; private set; }

    /// <summary>
    /// Gets the manufacturer or brand of the car.
    /// </summary>
    public string Make { get; private set; } = null!;

    /// <summary>
    /// Gets the model of the car.
    /// </summary>
    public string Model { get; private set; } = null!;

    /// <summary>
    /// Gets the manufacturing year of the car.
    /// </summary>
    public int Year { get; private set; }

    /// <summary>
    /// Gets the number of seats in the car.
    /// </summary>
    public int SeatsCount { get; private set; }

    /// <summary>
    /// Gets the daily rental rate for the car.
    /// </summary>
    public decimal DailyRate { get; private set; }

    /// <summary>
    /// Gets the type of transmission in the car (e.g., manual or automatic).
    /// </summary>
    public TransmissionType TransmissionType { get; private set; }

    /// <summary>
    /// Gets the type of fuel the car uses (e.g., gasoline, diesel, electric).
    /// </summary>
    public FuelType FuelType { get; private set; }

    /// <summary>
    /// Gets the car type associated with this car.
    /// </summary>
    public CarType? CarType { get; }

    /// <summary>
    /// Calculates the total rental price for the car based on the rental period and an optional one-way fee.
    /// </summary>
    /// <param name="period">The rental period, as defined by <see cref="RentalPeriod"/>.</param>
    /// <param name="oneWayFee">The optional one-way fee to be added to the total price. Default is 0.</param>
    /// <returns>The total rental price as a <see cref="decimal"/>.</returns>
    public decimal CalculateTotalPrice(RentalPeriod period, decimal oneWayFee = 0)
        => DailyRate * period.Days + oneWayFee;
}
