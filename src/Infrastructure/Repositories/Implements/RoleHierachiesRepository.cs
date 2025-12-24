namespace Infrastructure;

[RegisterService(typeof(IRoleHierachiesRepository))]
public class RoleHierachiesRepository(EfApplicationDbContext context) : RepositoryBase<RoleHierarchyEntity>(context), IRoleHierachiesRepository
{
    private readonly EfApplicationDbContext _context = context;
    public async Task<List<RoleLevelResponse>> GetRoleTreeByLevelAsync()
    {
        var roles = await _context.Roles
         .Where(r => _context.RoleHierarchies.Any(h => h.ParentRoleId == r.Id || h.ChildRoleId == r.Id))
         .ToListAsync();
        var hierarchies = await _context.RoleHierarchies.ToListAsync();

        var roleMap = roles.ToDictionary(r => r.Id, r => r.Name);

        // Lấy ra các role không bị làm Child → gốc
        var allChildIds = hierarchies.Select(h => h.ChildRoleId).ToHashSet();
        var rootRoleIds = roles.Select(r => r.Id).Where(id => !allChildIds.Contains(id)).ToList();

        var visited = new HashSet<Guid>();
        var queue = new Queue<(Guid roleId, int level)>();

        // Bắt đầu từ gốc
        foreach (var rootId in rootRoleIds)
        {
            queue.Enqueue((rootId, 0));
            visited.Add(rootId);

        }

        var levelMap = new Dictionary<int, List<Role>>();

        while (queue.Count > 0)
        {
            var (currentId, level) = queue.Dequeue();

            if (!levelMap.ContainsKey(level))
                levelMap[level] = new();
            var hierarchyMap = hierarchies
                .GroupBy(h => h.ChildRoleId)
                .ToDictionary(g => g.Key, g => g.First());
            levelMap[level].Add(new Role
            {
                Id = currentId,
                Name = roleMap.GetValueOrDefault(currentId) ?? "",
                ParentRoleId = hierarchyMap.GetValueOrDefault(currentId)?.ParentRoleId,
                HierarchyId = hierarchyMap.GetValueOrDefault(currentId)?.Id
            });

            var children = hierarchies
                .Where(h => h.ParentRoleId == currentId)
                .Select(h => h.ChildRoleId)
                .Where(childId => visited.Add(childId))
                .ToList();

            foreach (var childId in children)
            {
                queue.Enqueue((childId, level + 1));
            }
        }

        var result = levelMap
            .OrderBy(x => x.Key)
            .Select(x => new RoleLevelResponse
            {
                Level = x.Key,
                Roles = x.Value
            })
            .ToList();

        return result;
    }
}
