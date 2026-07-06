using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Singleton because the in-memory store must persist across requests.
        services.AddSingleton<IDonationRepository, InMemoryDonationRepository>();
        return services;
    }
}
