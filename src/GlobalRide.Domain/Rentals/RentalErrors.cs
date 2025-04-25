#pragma warning disable S3963

using System.Globalization;
using System.Resources;

using GlobalRide.Domain.Common.Result;

namespace GlobalRide.Domain.Rentals;

/// <summary>
/// Provides a collection of predefined validation errors specific to rental operations.
/// </summary>
public static class RentalErrors
{
    /// <summary>
    /// Represents a validation error that occurs when the dropoff branch is not allowed for the rental.
    /// </summary>
    public static readonly AppError DropoffBranchNotAllowed;

    /// <summary>
    /// Represents a validation error that occurs when the country pair (origin and destination) is not allowed for the rental.
    /// </summary>
    public static readonly AppError CountryPairNotAllowed;

    /// <summary>
    /// Represents a validation error that occurs when the rental period for a one-way rental is less than one day.
    /// </summary>
    public static readonly AppError OneWayRentalPeriodLessThanOneDay;

    /// <summary>
    /// Represents a validation error that occurs when the dropoff branch's capacity is exceeded for the rental.
    /// </summary>
    public static readonly AppError DropoffBranchCapacityExceeded;

    private static readonly ResourceManager ResourceManager;

    static RentalErrors()
    {
        ResourceManager = new ResourceManager(
            "GlobalRide.Domain.Rentals.Resources.Rentals",
            typeof(RentalErrors).Assembly);

        DropoffBranchNotAllowed = AppError.Validation(
            code: "Rental.DropoffBranchNotAllowed",
            message: GetMessage("Rental.DropoffBranchNotAllowed"));

        CountryPairNotAllowed = AppError.Validation(
            code: "Rental.CountryPairNotAllowed",
            message: GetMessage("Rental.CountryPairNotAllowed"));

        OneWayRentalPeriodLessThanOneDay = AppError.Validation(
            code: "Rental.OneWayRentalPeriodLessThanOneDay",
            message: GetMessage("Rental.OneWayRentalPeriodLessThanOneDay"));

        DropoffBranchCapacityExceeded = AppError.Validation(
            code: "Rental.DropoffBranchCapacityExceeded",
            message: GetMessage("Rental.DropoffBranchCapacityExceeded"));
    }

    private static string GetMessage(string code)
    {
        return ResourceManager.GetString(code, CultureInfo.CurrentUICulture)
            ?? throw new KeyNotFoundException("Validation message not found.");
    }
}
