using Training.AI.Domain.Repositories;
using Training.AI.Domain.Services;
using Training.AI.Grpc;
using Training.AI.Infrastructure.Factories;
using Training.AI.Infrastructure.Repositories;
using Training.AI.Infrastructure.Services;
using Training.AI.Interceptors;

namespace Training.AI;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });
        services.AddGrpcReflection();
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
    }

    public void Configure(WebApplication app)
    {
        app.MapGrpcService<AiGrpcService>();
        app.MapGrpcReflectionService();
        app.MapGet("/health", () => "OK");
    }
}
