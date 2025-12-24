namespace Application;

public interface IRoleHierachiesService
{
    Task<List<RoleLevelResponse>> GetRoleTreeByLevelAsync();
    Task<OperationResult> AddRoleHierarchieAsync(AddOrUpdateRoleHierarchieModel model);
    Task<OperationResult> UpdateRoleHierarchieAsync(Guid id, AddOrUpdateRoleHierarchieModel model);
    Task<OperationResult> DeleteRoleHierarchyAsync(Guid id);
}
