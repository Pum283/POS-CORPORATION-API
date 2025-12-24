namespace Infrastructure;

[RegisterService(typeof(IRolesRepository))]
public class RolesRepository(
    EfApplicationDbContext context,
    IMapper _mapper) : RepositoryBase<RoleEntity>(context), IRolesRepository
{
    private readonly EfApplicationDbContext _context = context;


    public Task<List<Role>> GetAllWithPagingAsync(QueryInfo queryInfo)
    {
        throw new NotImplementedException();
    }

    public async Task<QueryResult<GroupedPermissionModel>> GetPermissionsAsync(QueryInfo queryInfo)
    {
        var query = _context.Permissions
           .AsNoTracking()
           .GroupBy(p => p.Resource)
           .Select(g => new GroupedPermissionModel
           {
               Resource = g.Key,
               Actions = g.Select(p => new PermissionActionModel
               {
                   Action = p.Action,
                   PermissionId = p.Id
               }).ToList()
           });

        var entitiesPaged = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var totalCount = queryInfo.NeedTotalCount
            ? await query.CountAsync()
            : 0;

        return new QueryResult<GroupedPermissionModel>
        {
            Data = entitiesPaged,
            TotalCount = totalCount
        };
    }

    public async Task<List<Permission>> GetPermissionsForUserAsync(Guid userId)
    {
        var userRoles = await _context.UserRoles
        .Where(ur => ur.UserId == userId)
        .Select(ur => ur.RoleId)
        .ToListAsync();

        var allRoleIds = new HashSet<Guid>(userRoles);

        var visited = new HashSet<Guid>();
        var queue = new Queue<Guid>(userRoles);

        while (queue.Count > 0)
        {
            var roleId = queue.Dequeue();
            if (visited.Contains(roleId)) continue;
            visited.Add(roleId);

            var children = await _context.RoleHierarchies
                .Where(rh => rh.ParentRoleId == roleId)
                .Select(rh => rh.ChildRoleId)
                .ToListAsync();

            foreach (var childId in children)
            {
                if (!allRoleIds.Contains(childId))
                {
                    allRoleIds.Add(childId);
                    queue.Enqueue(childId);
                }
            }
        }

        var permissions = await _context.RolePermissions
            .Where(rp => allRoleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission)
            .Distinct()
            .ToListAsync();

        var result = _mapper.Map<List<Permission>>(permissions);

        return result;
    }


    public async Task<QueryResult<GroupedPermissionModel>> GetPermissionsForRoleAsync(QueryInfo queryInfo, Guid roleId)
    {
        // 1. Tìm toàn bộ RoleId cần lấy quyền (kể cả con)
        var allRoleIds = new HashSet<Guid>();
        var visited = new HashSet<Guid>();
        var queue = new Queue<Guid>();

        queue.Enqueue(roleId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!visited.Add(current)) continue;

            allRoleIds.Add(current);

            var childRoles = await _context.RoleHierarchies
                .Where(rh => rh.ParentRoleId == current)
                .Select(rh => rh.ChildRoleId)
                .ToListAsync(); 

            foreach (var child in childRoles)
            {
                if (!visited.Contains(child))
                    queue.Enqueue(child);
            }
        }

        var grouped = _context.RolePermissions
            .Where(rp => allRoleIds.Contains(rp.RoleId))
            .Select(rp => new
            {
                rp.Permission!.Resource,
                rp.Permission.Action,
                rp.Permission.Id
            })
            .AsNoTracking()
            .GroupBy(p => p.Resource)
            .Select(g => new GroupedPermissionModel
            {
                Resource = g.Key,
                Actions = g.Select(p => new PermissionActionModel
                {
                    Action = p.Action,
                    PermissionId = p.Id
                }).ToList()
            }); 

        var page = await grouped
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var totalCount = queryInfo.NeedTotalCount
            ? await grouped.CountAsync()
            : 0;


        return new QueryResult<GroupedPermissionModel>
        {
            Data = grouped,
            TotalCount = totalCount
        };
    }


    public async Task<List<Role>> GetRoleForUserAsync(Guid userId)
    {
        var userRoles = await _context.UserRoles
         .Where(ur => ur.UserId == userId)
         .Select(ur => new Role
         {
             Id = ur.Role.Id,
             Name = ur.Role.Name
         }).ToListAsync();

        return userRoles;
    }
    public async Task<List<Guid>> GetAllAncestorRoleIdsAsync(Guid childRoleId)
    {
        var visited = new HashSet<Guid>();
        var toVisit = new Queue<Guid>();
        toVisit.Enqueue(childRoleId);

        while (toVisit.Any())
        {
            var current = toVisit.Dequeue();

            // Lấy danh sách các cha của current
            var parents = await _context.RoleHierarchies
                .Where(rh => rh.ChildRoleId == current)
                .Select(rh => rh.ParentRoleId)
                .ToListAsync();

            foreach (var parentId in parents)
            {
                if (!visited.Contains(parentId))
                {
                    visited.Add(parentId);
                    toVisit.Enqueue(parentId); // tiếp tục tìm cha của cha
                }
            }
        }

        return visited.ToList();
    }

    public async Task<List<Guid>> GetPermissionIdsByRoleAsync(Guid roleId)
    {
        return await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync();
    }

    public async Task<List<Guid>> GetPermissionIdsByRolesAsync(List<Guid> roleIds)
    {
        return await _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.PermissionId)
            .ToListAsync();
    }
    public async Task<Dictionary<Guid, List<Guid>>> GetAllAncestorsForRolesAsync(List<Guid> roleIds)
    {
        var hierarchy = await _context.RoleHierarchies.ToListAsync();
        var map = roleIds.ToDictionary(id => id, _ => new List<Guid>());

        foreach (var roleId in roleIds)
        {
            var visited = new HashSet<Guid>();
            var queue = new Queue<Guid>();
            queue.Enqueue(roleId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var parents = hierarchy
                    .Where(h => h.ChildRoleId == current)
                    .Select(h => h.ParentRoleId)
                    .Where(pid => visited.Add(pid))
                    .ToList();

                foreach (var parent in parents)
                {
                    map[roleId].Add(parent);
                    queue.Enqueue(parent);
                }
            }
        }

        return map;
    }
    public async Task<Dictionary<Guid, HashSet<Guid>>> GetPermissionsForRolesMappedAsync(List<Guid> roleIds)
    {
        return await _context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .GroupBy(rp => rp.RoleId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Select(rp => rp.PermissionId).ToHashSet()
            );
    }

    

    public async Task<HashSet<Guid>> GetAllAncestorsAsync(Guid roleId)
    {
        var result = new HashSet<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(roleId);

        var all = await _context.RoleHierarchies.ToListAsync();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            var parents = all
                .Where(h => h.ChildRoleId == current)
                .Select(h => h.ParentRoleId)
                .Where(pid => result.Add(pid))
                .ToList();

            foreach (var parent in parents)
                queue.Enqueue(parent);
        }

        return result;
    }
}
