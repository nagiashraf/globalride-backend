using GlobalRide.Domain.Common;

namespace GlobalRide.Domain.Branches;

/// <summary>
/// Represents the response for searching for branches.
/// </summary>
public sealed record class BranchSearchResultResponse(
    Guid Id,
    LanguageCode LanguageCode,
    string Name,
    string City,
    string Country,
    BranchType Type,
    string TimeZone);
