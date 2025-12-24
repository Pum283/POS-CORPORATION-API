namespace Infrastructure;

public interface IRolesRepository : IRepositoryBase<RoleEntity>
{
    Task<List<Role>> GetAllWithPagingAsync(QueryInfo queryInfo);
    Task<QueryResult<GroupedPermissionModel>> GetPermissionsAsync(QueryInfo queryInfo);
    Task<List<Permission>> GetPermissionsForUserAsync(Guid userId);
    Task<QueryResult<GroupedPermissionModel>> GetPermissionsForRoleAsync(QueryInfo queryInfo, Guid roleId);
    Task<List<Role>> GetRoleForUserAsync(Guid userId);
    Task<List<Guid>> GetAllAncestorRoleIdsAsync(Guid childRoleId);
    Task<List<Guid>> GetPermissionIdsByRoleAsync(Guid roleId);
    Task<List<Guid>> GetPermissionIdsByRolesAsync(List<Guid> roleIds);
    Task<Dictionary<Guid, List<Guid>>> GetAllAncestorsForRolesAsync(List<Guid> roleIds);
    Task<HashSet<Guid>> GetAllAncestorsAsync(Guid roleId);
    Task<Dictionary<Guid, HashSet<Guid>>> GetPermissionsForRolesMappedAsync(List<Guid> roleIds);
}
