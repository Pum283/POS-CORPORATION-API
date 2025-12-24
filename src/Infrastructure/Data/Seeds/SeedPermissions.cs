namespace Infrastructure;

public static class PermissionSeeder
{
    public static void SeedPermissions(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EfApplicationDbContext>();

        var allPermissions = Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetMethods())
            .SelectMany(m => m.GetCustomAttributes<RequirePermissionsAttribute>())
            .SelectMany(attr => attr.RequiredPermissions) 
            .Distinct()
            .Select(p =>
            {
                var parts = p.Split(':');
                return new { Resource = parts[0], Action = parts[1] };
            });

        foreach (var perm in allPermissions)
        {
            if (!db.Permissions.Any(p => p.Resource == perm.Resource && p.Action == perm.Action))
            {
                db.Permissions.Add(new PermissionEntity
                {
                    Id = Guid.NewGuid(),
                    Resource = perm.Resource,
                    Action = perm.Action
                });
            }
        }
        db.SaveChanges();
    }
}