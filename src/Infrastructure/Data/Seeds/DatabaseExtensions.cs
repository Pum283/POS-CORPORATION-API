using Microsoft.AspNetCore.Builder;

namespace Infrastructure;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<EfApplicationDbContext>();

        context.Database.MigrateAsync().GetAwaiter().GetResult();

        await SeedAsync(context);
    }

    private static async Task SeedAsync(EfApplicationDbContext context)
    {
        await SeedRoleAsync(context);
        await SeedCustomerAsync(context);
        await SeedUserRolesAsync(context);
        await SeedStoresAsync(context);
        await SeedStoreAccountantsAsync(context);
        await SeedProductCategoriesAsync(context);
        await SeedProductGroupsAsync(context);
        await SeedProductsAsync(context);
        await SeedPermissionsAsync(context);
    }
    private static async Task SeedRoleAsync(EfApplicationDbContext context)
    {
        if (!await context.Roles.AnyAsync())
        {
            await context.Roles.AddRangeAsync(InitialData.Roles);
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedCustomerAsync(EfApplicationDbContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            await context.Users.AddRangeAsync(InitialData.Users);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedUserRolesAsync(EfApplicationDbContext context)
    {
        if (!await context.UserRoles.AnyAsync())
        {
            await context.UserRoles.AddRangeAsync(InitialData.UserRoles);
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedStoresAsync(EfApplicationDbContext context)
    {
        if (!await context.Stores.AnyAsync())
        {
            await context.Stores.AddRangeAsync(InitialData.Stores);
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedStoreAccountantsAsync(EfApplicationDbContext context)
    {
        if (!await context.StoreAccountants.AnyAsync())
        {
            await context.StoreAccountants.AddRangeAsync(InitialData.StoreAccountants);
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedProductCategoriesAsync(EfApplicationDbContext context)
    {
        if (!await context.ProductCategoies.AnyAsync())
        {
            await context.ProductCategoies.AddRangeAsync(InitialData.ProductCategories);
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedProductGroupsAsync(EfApplicationDbContext context)
    {
        if (!await context.ProductGroups.AnyAsync())
        {
            await context.ProductGroups.AddRangeAsync(InitialData.ProductGroups);
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedProductsAsync(EfApplicationDbContext context)
    {
        if (!await context.Products.AnyAsync())
        {
            await context.Products.AddRangeAsync(InitialData.Products);
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedPermissionsAsync(EfApplicationDbContext context)
    {
        if (!await context.Permissions.AnyAsync())
        {
            await context.Permissions.AddRangeAsync(InitialData.Permissions);
            await context.SaveChangesAsync();
        }
    }
}
