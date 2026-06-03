using Training.AI.Domain.Repositories;
using Training.AI.Domain.Services;
using Training.AI.Infrastructure.Factories;
using Training.AI.Infrastructure.Repositories;
using Training.AI.Infrastructure.Services;

namespace Training.AI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IGenerationHistoryRepository, GenerationHistoryRepository>();
        services.AddHttpClient<IAiPlanGenerator, OpenAiPlanGenerator>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
