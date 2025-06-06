using BlazorSortable.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorSortable;

/// <summary>
/// Provides extension methods for configuring Sortable services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the necessary services for Sortable functionality to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddSortableServices(this IServiceCollection services)
    {
        services.AddScoped<ISortableService, SortableService>();

        return services;
    }
}
