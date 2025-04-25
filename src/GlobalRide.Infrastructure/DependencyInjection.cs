using GlobalRide.Domain.AllowedCountryPairs;
using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Cars;
using GlobalRide.Domain.Common;
using GlobalRide.Infrastructure.FullTextSearch.LuceneNet;
using GlobalRide.Infrastructure.Persistence;
using GlobalRide.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalRide.Infrastructure;

/// <summary>
/// Provides extension methods for setting up the infrastructure project services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the infrastructure project services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="connectionString">The connection string used to connect to the database.</param>
    /// <param name="rootPath">The root path used for Lucene searching service.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString, string rootPath)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IBranchRepository, BranchRepository>();

        services.AddScoped<ICarRepository, CarRepository>();

        services.AddScoped<IAllowedCountryPairRepository, AllowedCountryPairRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        string indexesDirectoryPath = Path.Combine(rootPath, "Setup", "FullTextSearchIndexes", "Branches");
        services.AddSingleton<IBranchSearchingService>(new LuceneBranchSearchingService(indexesDirectoryPath));

        return services;
    }
}
