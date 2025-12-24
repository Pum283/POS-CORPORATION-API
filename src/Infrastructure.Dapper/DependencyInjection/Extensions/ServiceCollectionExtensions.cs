using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Dapper;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDapperInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAppConfiguration, AppConfiguration>();
        services.AddScoped<DapperApplicationDbContext>();

        services.AddScoped<IDapperUnitOfWork, DapperUnitOfWork>();

        return services;
    }
}
