using System.Reflection;
using Application.BackgroundJobs;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using StackExchange.Redis;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<NotificationHelper>();
        services.AddServicesFromAssembly(
            Assembly.GetEntryAssembly() ?? Assembly.Load(""),
            Assembly.Load("application"),
            Assembly.Load("infrastructure"),
            Assembly.Load("infrastructure.dapper")
        );
        services.AddBlobStorageService(configuration);
        services.AddCacheService(configuration);
        services.AddMemoryCache();
        services.AddEmailService(configuration);

        services.AddScoped(typeof(IExcelExporter<>), typeof(ClosedXMLExcelExporter<>));
        return services;
    }
    public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            if (assembly == null) continue;

            var types = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.GetCustomAttribute<RegisterServiceAttribute>() != null)
                .ToList();

            services.ScanTypes(types);
        }
        return services;
    }

    public static IServiceCollection ScanTypes(this IServiceCollection services, List<Type> types)
    {
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<RegisterServiceAttribute>();

            if (attribute == null)
            {
                continue;
            }

            // Register service
            switch (attribute.Lifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient(attribute.ServiceType, type);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(attribute.ServiceType, type);
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(attribute.ServiceType, type);
                    break;
            }
        }
        return services;
    }

    public static IServiceCollection AddBlobStorageService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new BlobServiceClient(configuration.GetConnectionString(AppConstants.AzureBlobStorage)));

        services.AddScoped<IBlobStorageService, BlobStorageService>();
        return services;
    }
    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString(AppConstants.RedisConnection)
            ?? throw new ParameterInvalidException("");

        var options = ConfigurationOptions.Parse(redisConnectionString);
        options.Ssl = false;                 // Upstash yêu cầu SSL
        options.AbortOnConnectFail = false; // tránh throw khi không connect ngay

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(options));

        services.AddScoped<IRedisService, RedisService>();


        return services;
    }
    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        var mailSettingsSection = configuration.GetSection("MailSettings");
        services.Configure<EmailSettings>(mailSettingsSection);
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(InventoryAlertJob));
            q.AddJob<InventoryAlertJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("InventoryAlertJob-trigger")
                //.WithCronSchedule("0 0/3 * * * ?")) ; // chạy 3 phút 1 lần để test
                //.WithCronSchedule("0 30 14 * * ?"));  // Job sẽ chạy mỗi ngày vào lúc 14:30 (2:30 chiều).
                .WithCronSchedule("0 30 14 * * ?", x =>
                x.WithMisfireHandlingInstructionFireAndProceed()
                 .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"))));


            // Job mới: AutoInventoryPeriodLockJob
            var autoLockJobKey = new JobKey(nameof(AutoInventoryPeriodLockJob));
            q.AddJob<AutoInventoryPeriodLockJob>(opts => opts.WithIdentity(autoLockJobKey));
            q.AddTrigger(opts => opts
                .ForJob(autoLockJobKey)
                .WithIdentity("AutoInventoryPeriodLockJob-trigger")
                //.WithCronSchedule("0 0/3 * * * ?")); // chạy 3 phút 1 lần để test
                //.WithCronSchedule("0 00 23 * * ?")); // chạy lúc 23:00 mỗi ngày
                .WithCronSchedule("0 30 23 * * ?", x =>
                x.WithMisfireHandlingInstructionFireAndProceed()
                 .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"))));

            Console.WriteLine("✅ Quartz jobs registered successfully.");
        });
        // Đăng ký background service cho Quartz
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        return services;
    }
}
