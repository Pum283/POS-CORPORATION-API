using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddRateLimiting(builder.Configuration);

builder.Services.AddCorsPolicy()
                .AddJwtAuthentication(builder.Configuration)
                .AddFluentValidation()
                .AddAutoMapper(typeof(AutoMapperProfiles))
                .AddDapperInfrastructureServices(builder.Configuration)
                .AddInfrastructureServices(builder.Configuration)
                .AddApplicationServices(builder.Configuration)
                .AddApiServices()
                .AddBackgroundJobs(builder.Configuration);

builder.Services.AddConfigureMediatR();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddTransient<TokenValidationMiddleware>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitializeDatabaseAsync();
}
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthorization();

app.UseIpRateLimiting();

app.UseMiddleware<PermissionsMiddleware>();
app.UseMiddleware<CustomIpRateLimitMiddleware>();

app.MapAllHubs("/hubs");

app.MapControllers();
PermissionSeeder.SeedPermissions(app.Services);
try
{
    await app.RunAsync();
    Log.Information("Stopped cleanly.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during boostrapping.");
    await app.StopAsync();
}
finally
{
    Log.CloseAndFlush();
    await app.DisposeAsync();
}