using GlobalRide.Domain.Common;

namespace GlobalRide.Domain.Branches;

/// <summary>
/// Represents a translation for a branch entity.
/// </summary>
public sealed record class BranchTranslation(
    LanguageCode LanguageCode,
    string Name,
    string City,
    string Country);
