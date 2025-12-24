namespace Application;

[RegisterService(typeof(IPermissionService))]
public class PermissionService : IPermissionService
{
    private readonly IRolesRepository _rolesRepository;
    private readonly IRedisService _redisService;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public PermissionService(IRolesRepository rolesRepository, IRedisService redisService)
    {
        _rolesRepository = rolesRepository;
        _redisService = redisService;
    }

    public async Task<List<string>> GetPermissionsForUserAsync(Guid userId)
    {
        var key = $"permissions:user:{userId}";

        // lấy cache hiện tại
        var cached = await _redisService.GetAsync<List<string>>(key);

        // lấy DB hiện tại
        var dbPermissions = await _rolesRepository.GetPermissionsForUserAsync(userId);
        var dbList = dbPermissions.Select(p => $"{p.Resource}:{p.Action}").ToList();

        // nếu cache null hoặc số lượng khác DB → update lại cache
        if (cached == null || cached.Count != dbList.Count)
        {
            await _redisService.SetAsync(key, dbList, _cacheDuration);
            return dbList;
        }

        return cached;
    }

    public async Task InvalidatePermissionsAsync(Guid userId)
    {
        var key = $"permissions:user:{userId}";
        await _redisService.DeleteAsync(key);
    }
}
