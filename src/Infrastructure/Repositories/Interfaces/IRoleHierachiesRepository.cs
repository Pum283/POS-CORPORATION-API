namespace Infrastructure;

public interface IRoleHierachiesRepository : IRepositoryBase<RoleHierarchyEntity>
{
    Task<List<RoleLevelResponse>> GetRoleTreeByLevelAsync();

}
