namespace Application;

public interface IRolesService
{
    Task<List<Role>> GetRolesAsync();
    Task<QueryResult<GroupedPermissionModel>> GetPermissionsAsync(QueryInfo queryInfo);
    Task<UserForTokenModel?> GetUserPermissionsAsync(Guid id);
    Task<List<Permission>> GetPermissionsForUserAsync(Guid userId);
    Task<QueryResult<GroupedPermissionModel>> GetPermissionsForRoleAsync(QueryInfo queryInfo, Guid roleId);
    Task<List<Role>> GetRoleForUserAsync(Guid userId);
    Task<OperationResult> CreateRoleAsync(CreateOrUpdateRoleModel model);
    Task<OperationResult> UpdateRoleAsync(Guid roleId, CreateOrUpdateRoleModel model);
    Task<OperationResult> DeleteRoleAsync(Guid roleId);
    Task<OperationResult> DeletePermisionForRoleAsync(Guid roleId);
    Task<OperationResult> AddPermissionsForRole(AddPermissionsForRoleModel model);
    Task<OperationResult> AddPermissionsForRoles(List<AddPermissionsForRoleModel> models);

}
