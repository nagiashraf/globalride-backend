using FluentValidation;

using GlobalRide.Application.Common.Behaviors;
using GlobalRide.Domain.Rentals;

using Microsoft.Extensions.DependencyInjection;

namespace GlobalRide.Application;

/// <summary>
/// Provides extension methods for setting up the application project services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the application project services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        services.AddTransient<IOneWayRentalService, OneWayRentalService>();

        return services;
    }
}
