namespace API.Controllers;

[Route("rolehierachies")]
public class RoleHierachiesController(IRoleHierachiesService _service) : BaseApiController
{
    [HttpGet]
    public async Task<List<RoleLevelResponse>> GetRoleTreeByLevel()
    {
        return await _service.GetRoleTreeByLevelAsync();
    }
    [HttpPost]
    [ValidateModel]
    public async Task<OperationResult> AddRoleHierarchy([FromBody] AddOrUpdateRoleHierarchieModel model)
    {
        return await _service.AddRoleHierarchieAsync(model);
    }
    [HttpPatch("{id}")]
    [ValidateModel]
    public async Task<OperationResult> UpdateRoleHierarchy([FromRoute] Guid id, [FromBody] AddOrUpdateRoleHierarchieModel model)
    {
        return await _service.UpdateRoleHierarchieAsync(id, model);
    }
    [HttpDelete("{id}")]
    public async Task<OperationResult> DeleteRoleHierarchy([FromRoute] Guid id)
    {
       return await _service.DeleteRoleHierarchyAsync(id);
    }
}
