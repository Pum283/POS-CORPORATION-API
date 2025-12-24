namespace API.Controllers;

[Route("roles")]
public class RolesController(
    IRolesService _service) : BaseApiController
{
    [HttpGet]
    public async Task<List<Role>> GetRoles()
    {
        return await _service.GetRolesAsync();
    }
    [HttpGet("grouped-permissions")]
    public async Task<QueryResult<GroupedPermissionModel>> GetPermissions([FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetPermissionsAsync(queryInfo);
    }
    [HttpGet("permissions/{userId}")]
    public async Task<List<Permission>> GetPermissionsForUser(Guid userId)
    {
        return await _service.GetPermissionsForUserAsync(userId);
    }
    [HttpGet("permissions/{roleId}/role")]
    public async Task<QueryResult<GroupedPermissionModel>> GetPermissionsForRole([FromQuery] QueryInfo queryInfo, [FromRoute] Guid roleId)
    {
        return await _service.GetPermissionsForRoleAsync(queryInfo, roleId);
    }
    [HttpGet("{id}")]
    public async Task<List<Role>> GetRolesForUser(Guid id)
    {
        return await _service.GetRoleForUserAsync(id);
    }

    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> CreateRole([FromBody] CreateOrUpdateRoleModel model)
    {
        return await _service.CreateRoleAsync(model);
    }
    [HttpPost("add-permission")]
    [ValidateModel]
    public async Task<OperationResult> AddPermissionsForRole([FromBody] AddPermissionsForRoleModel model)
    {
        return await _service.AddPermissionsForRole(model);
    }
    [HttpPost("add-permission-bulk")]
    [ValidateModel]
    public async Task<OperationResult> AddPermissionsForRoles([FromBody] List<AddPermissionsForRoleModel> models)
    {
        return await _service.AddPermissionsForRoles(models);
    }
    [HttpPatch("{roleId}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateRole([FromRoute] Guid roleId, [FromBody] CreateOrUpdateRoleModel model)
    {
        return await _service.UpdateRoleAsync(roleId, model);
    }
    [HttpDelete("{roleId}")]
    public async Task<OperationResult> DeleteRole([FromRoute] Guid roleId)
    {
        return await _service.DeleteRoleAsync(roleId);
    }
    [HttpDelete("{permissionId}/permission")]
    public async Task<OperationResult> DeletePermisionForRole([FromRoute] Guid permissionId)
    {
        return await _service.DeletePermisionForRoleAsync(permissionId);
    }
}
