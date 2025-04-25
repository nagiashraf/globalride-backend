using GlobalRide.Domain.Common;

namespace GlobalRide.Domain.Branches;

/// <summary>
/// Represents a branch entity in the domain.
/// </summary>
public sealed class Branch : Entity<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Branch"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the branch.</param>
    /// <param name="countryCode">The country code of the branch.</param>
    /// <param name="timeZone">The time zone of the branch city.</param>
    /// <param name="capacity">The maximum number of cars that can be stored at the branch..</param>
    /// <param name="isOneWayDropoffAllowed">Indicates whether one-way dropoff is allowed at the branch.</param>
    /// <param name="type">The type of the branch, as defined by the <see cref="BranchType"/> enum.</param>
    /// <param name="coordinate">The geographic coordinates of the branch.</param>
    /// <param name="translations">A collection of translations associated with the branch.</param>
    public Branch(
        Guid id,
        string countryCode,
        string timeZone,
        int capacity,
        bool isOneWayDropoffAllowed,
        BranchType type,
        Coordinate coordinate,
        IReadOnlyCollection<BranchTranslation> translations)
        : base(id)
    {
        CountryCode = countryCode.ToUpperInvariant();
        TimeZone = timeZone;
        Capacity = capacity;
        IsOneWayDropoffAllowed = isOneWayDropoffAllowed;
        Coordinate = coordinate;
        Type = type;
        Translations = translations;
    }

    private Branch()
    {
    }

    /// <summary>
    /// Gets the country code of the branch.
    /// </summary>
    public string CountryCode { get; private set; } = null!;

    /// <summary>
    /// Gets the time zone of the branch city. The value should be an IANA time zone name (e.g., "America/New_York"): https://en.wikipedia.org/wiki/List_of_tz_database_time_zones.
    /// </summary>
    public string TimeZone { get; private set; } = null!;

    /// <summary>
    /// Gets the type of the branch.
    /// </summary>
    public BranchType Type { get; private set; }

    /// <summary>
    /// Gets the capacity of the branch.
    /// </summary>
    public int Capacity { get; private set; }

    /// <summary>
    /// Gets a value indicating whether one-way dropoff is allowed at the branch.
    /// </summary>
    public bool IsOneWayDropoffAllowed { get; private set; }

    /// <summary>
    /// Gets the geographic coordinates of the branch.
    /// </summary>
    public Coordinate Coordinate { get; private set; } = null!;

    /// <summary>
    /// Gets the collection of translations associated with the branch.
    /// </summary>
    public IReadOnlyCollection<BranchTranslation> Translations { get; } = null!;
}
