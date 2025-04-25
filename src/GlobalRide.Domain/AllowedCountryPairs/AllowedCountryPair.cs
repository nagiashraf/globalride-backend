namespace GlobalRide.Domain.AllowedCountryPairs;

/// <summary>
/// Represents a pair of allowed countries for a rental.
/// </summary>
public sealed class AllowedCountryPair
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllowedCountryPair"/> class.
    /// </summary>
    /// <param name="countryCodeFrom">The country code from which the rental is allowed.</param>
    /// <param name="countryCodeTo">The country code to which the rental is allowed.</param>
    public AllowedCountryPair(
        string countryCodeFrom,
        string countryCodeTo)
    {
        CountryCodeFrom = countryCodeFrom;
        CountryCodeTo = countryCodeTo;
    }

    private AllowedCountryPair()
    {
    }

/// <summary>
/// Gets the country code from which the rental is allowed.
/// </summary>
    public string CountryCodeFrom { get; private set; } = null!;

/// <summary>
/// Gets the country code to which the rental is allowed.
/// </summary>
    public string CountryCodeTo { get; private set; } = null!;
}
