using GlobalRide.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace GlobalRide.Infrastructure.IntegrationTests.Persistence;

public class DatabaseFixture
{
    private const string ConnectionString =
        @"server=.;database=GlobalRideTestsDB;Trusted_Connection=true;TrustServerCertificate=true";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public DatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using var context = CreateContext();
                context.Database.EnsureCreated();

                _databaseInitialized = true;
            }
        }
    }

    public AppDbContext CreateContext()
        => new(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);
}
