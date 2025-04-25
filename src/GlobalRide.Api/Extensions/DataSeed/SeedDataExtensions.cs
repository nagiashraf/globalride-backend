using System.Text.Json;
using System.Text.Json.Serialization;

using GlobalRide.Domain.AllowedCountryPairs;
using GlobalRide.Domain.Branches;
using GlobalRide.Domain.Cars;
using GlobalRide.Domain.CarTypes;
using GlobalRide.Domain.Common;
using GlobalRide.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace GlobalRide.Api.Extensions.DataSeed;

    /// <summary>
    /// Seeds initial data into the database from JSON files located in the specified directory.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance to extend.</param>
    /// <param name="rootPath">The root path of the application, used to locate the seed data files.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method checks if the database tables for <see cref="Branch"/> and <see cref="Car"/> entities are empty.
    /// If they are, it reads seed data from JSON files and adds the data to the database. It also updates the branch
    /// search service with translations for branches.
    /// </remarks>
public static class SeedDataExtensions
{
    // Using a local instance of JsonSerializerOptions can substantially degrade performance if the code executes multiple times.
    // Use the singleton pattern to avoid creating a new JsonSerializerOptions instance every time code is executed.
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            new CoordinateJsonConverter(),
            new CarTypeJsonConverter(),
        },
    };

    /// <summary>
    /// Seeds initial data into the database from JSON files and updates full-text search indexes.
    /// </summary>
    /// <param name="app">The application builder instance.</param>
    /// <param name="rootPath">The root directory path of the application.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task SeedData(this IApplicationBuilder app, string rootPath)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var seedFilesDirectoryPath = Path.Combine(rootPath, "Setup", "SeedData");

        await SeedEntityAsync<Branch, Guid>(context, logger, seedFilesDirectoryPath, "branches.json");
        await SeedEntityAsync<CarType, Guid>(context, logger, seedFilesDirectoryPath, "carTypes.json");
        await SeedEntityAsync<Car, string>(context, logger, seedFilesDirectoryPath, "cars.json");
        await SeedAllowedCountryPairsAsync(context, logger, seedFilesDirectoryPath, "allowedCountryPairs.json");

        await context.SaveChangesAsync();

        var branchRepository = scope.ServiceProvider.GetRequiredService<IBranchRepository>();
        var branchSearchingService = scope.ServiceProvider.GetRequiredService<IBranchSearchingService>();

        var branchesTranslations = await branchRepository.ListTranslationsAsync();
        branchSearchingService.AddRange(branchesTranslations);
    }

    private static async Task SeedEntityAsync<TEntity, TId>(
        AppDbContext context,
        ILogger logger,
        string seedPath,
        string fileName)
        where TEntity : Entity<TId>
        where TId : notnull
    {
        var filePath = Path.Combine(seedPath, fileName);

        var set = context.Set<TEntity>();
        if (await set.AnyAsync())
        {
            logger.LogInformation("{EntityType} already exists in database. Skipping seeding.", typeof(TEntity).Name);
            return;
        }

        var entities = await GetDeserializedObjectsAsync<TEntity>(filePath);
        if (entities?.Count > 0)
        {
            set.AddRange(entities);
            logger.LogInformation("Seeding {Count} {EntityType} records...", entities.Count, typeof(TEntity).Name);
        }
    }

    private static async Task<List<T>?> GetDeserializedObjectsAsync<T>(string filePath)
    {
        string jsonObjects = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<List<T>?>(jsonObjects, JsonOptions);
    }

    private static async Task SeedAllowedCountryPairsAsync(
        AppDbContext context,
        ILogger logger,
        string seedPath,
        string fileName)
    {
        var filePath = Path.Combine(seedPath, fileName);

        var set = context.Set<AllowedCountryPair>();
        if (await set.AnyAsync())
        {
            logger.LogInformation("AllowedCountryPair already exists in database. Skipping seeding.");
            return;
        }

        var entities = await GetDeserializedObjectsAsync<AllowedCountryPair>(filePath);
        if (entities?.Count > 0)
        {
            set.AddRange(entities);
            logger.LogInformation("Seeding {Count} AllowedCountryPair records...", entities.Count);
        }
    }
}
