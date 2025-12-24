namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AuditableEntityInterceptor>();
        services.AddScoped<IAppConfiguration, AppConfiguration>();
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

        services.Configure<SqlServerRetryOptions>(configuration.GetSection("SqlServerRetryOptions"));

        services.AddDbContextPool<DbContext, EfApplicationDbContext>((provider, builder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var interceptor = provider.GetRequiredService<AuditableEntityInterceptor>();
            var options = provider.GetRequiredService<IOptionsMonitor<SqlServerRetryOptions>>();
            builder
                .EnableDetailedErrors(true)
                .EnableSensitiveDataLogging(true)
                //.UseLazyLoadingProxies(true)
                .UseSqlServer(
                    configuration.GetConnectionString(AppConstants.SqlServerConnection),
                    sqlServerOptionsAction => sqlServerOptionsAction
                        .EnableRetryOnFailure(
                            maxRetryCount: options.CurrentValue.MaxRetryCount,
                            maxRetryDelay: options.CurrentValue.MaxRetryDelay,
                            errorNumbersToAdd: options.CurrentValue.ErrorNumbersToAdd)
                )
                .AddInterceptors(interceptor);

        });

        return services;
    }
}
