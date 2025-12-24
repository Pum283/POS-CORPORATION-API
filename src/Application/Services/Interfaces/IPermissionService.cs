namespace Application;

public interface IPermissionService
{
    Task<List<string>> GetPermissionsForUserAsync(Guid userId);
    Task InvalidatePermissionsAsync(Guid userId);
}
