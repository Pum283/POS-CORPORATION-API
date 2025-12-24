namespace API;

public static class CorsServiceCollectionExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("http://localhost:3000", "https://localhost:3000", "http://103.90.226.74:3000", "https://pos-digifnb.vercel.app", "http://127.0.0.1:5500")
                       .AllowAnyMethod()
                       .SetPreflightMaxAge(TimeSpan.FromDays(1))
                       .SetIsOriginAllowed(origin => true)
                       .AllowCredentials()
                       .AllowAnyHeader()
                        ;
            });
        });
        return services;
    }
}
// http://localhost:6060