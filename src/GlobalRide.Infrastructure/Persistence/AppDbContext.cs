using GlobalRide.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace GlobalRide.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions options)
    : DbContext(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
