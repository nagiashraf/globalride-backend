using GlobalRide.Domain.Cars;

namespace GlobalRide.Application.Cars.SearchCars;

/// <summary>
/// Represents a response containing details about a car.
/// </summary>
/// <param name="Id">The unique identifier of the car.</param>
/// <param name="Make">The manufacturer or brand of the car.</param>
/// <param name="Model">The model of the car.</param>
/// <param name="Year">The manufacturing year of the car.</param>
/// <param name="SeatsCount">The number of seats in the car.</param>
/// <param name="PriceForPeriod">The total cost of renting the car.</param>
/// <param name="TransmissionType">The type of transmission in the car (e.g., manual or automatic).</param>
/// <param name="FuelType">The type of fuel the car uses (e.g., gasoline, diesel, electric).</param>
public record class CarResponse(
    string Id,
    string Make,
    string Model,
    int Year,
    int SeatsCount,
    decimal PriceForPeriod,
    TransmissionType TransmissionType,
    FuelType FuelType);
