#pragma warning disable SA1515

using GlobalRide.Domain.CarTypes;

namespace GlobalRide.Domain.UnitTests.CarTypes;

public class CarTypeTests
{
    public static TheoryData<Guid, string, bool, decimal?> ValidParametersTestCases =>
    new()
    {
        // Valid: one-way allowed with multiplier
        {
            Guid.NewGuid(),
            "SUV",
            true,
            1.5m
        },
        // Valid: one-way not allowed, no multiplier
        {
            Guid.NewGuid(),
            "Economy",
            false,
            null
        },
    };

    public static TheoryData<Guid, string, bool, decimal?> InvalidParametersTestCases =>
        new()
        {
            // Invalid: one-way allowed but no multiplier
            {
                Guid.NewGuid(),
                "SUV",
                true,
                null
            },
            // Invalid: one-way not allowed but has multiplier
            {
                Guid.NewGuid(),
                "Economy",
                false,
                1.5m
            },
        };

    [Theory]
    [MemberData(nameof(ValidParametersTestCases))]
    public void Create_ValidParameters_ReturnsSuccessResult(
        Guid id,
        string category,
        bool isOneWayAllowed,
        decimal? multiplier)
    {
        // Act
        var result = CarType.Create(id, category, isOneWayAllowed, multiplier);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [MemberData(nameof(InvalidParametersTestCases))]
    public void Create_InvalidParameters_ReturnsFailureResult(
        Guid id,
        string category,
        bool isOneWayAllowed,
        decimal? multiplier)
    {
        // Act
        var result = CarType.Create(id, category, isOneWayAllowed, multiplier);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
