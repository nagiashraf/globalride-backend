using GlobalRide.Domain.CarTypes;

namespace TestsCommon.CarTypes;

public static class CarTypeFactory
{
    public static CarType Create(
        Guid? id = null,
        string category = "SUV",
        bool isOneWayAllowed = true,
        decimal? multiplier = 1)
        => CarType.Create(
            id ?? Guid.NewGuid(),
            category,
            isOneWayAllowed,
            multiplier).Value;
}
