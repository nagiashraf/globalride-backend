using GlobalRide.Domain.Branches;

namespace TestsCommon.Branches;

public static class BranchFactory
{
    public static Branch Create(
        Guid? id = null,
        string countryCode = "US",
        string timeZone = "America/New_York",
        int capacity = 10,
        bool isOneWayDropoffAllowed = false,
        BranchType branchType = BranchType.Airport,
        Coordinate? coordinate = null,
        IReadOnlyCollection<BranchTranslation>? translations = null)
        => new (
            id ?? Guid.NewGuid(),
            countryCode,
            timeZone,
            capacity,
            isOneWayDropoffAllowed,
            branchType,
            coordinate ?? Coordinate.Create(0, 0).Value,
            translations ?? []);
}
