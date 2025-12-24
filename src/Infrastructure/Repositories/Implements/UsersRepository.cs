namespace Infrastructure;

[RegisterService(typeof(IUsersRepository))]
public class UsersRepository(
    EfApplicationDbContext context,
    IMapper _mapper) : RepositoryBase<UserEntity>(context), IUsersRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<QueryResult<User>> GetAllWithPagingAsync(QueryInfo queryInfo)
    {
        var query = from user in _context.Users.AsNoTracking()
                    where EF.Functions.Collate(user.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText ?? "")
                         && user.IsDeleted == queryInfo.IsActive
                    join store in _context.Stores.AsNoTracking()
                     on user.StoreId equals store.Id into storeGroup
                    from store in storeGroup.DefaultIfEmpty()
                    join role in _context.UserRoles.AsNoTracking()
                     on user.Id equals role.UserId into roleGroup

                    select new User
                    {
                        Id = user.Id,
                        StoreId = user.StoreId ?? Guid.Empty,
                        Email = user.Email,
                        Name = user.Name,
                        Picture = user.Picture,
                        Phone = user.Phone,
                        Role = roleGroup.Select(x => x.Role.Name).FirstOrDefault() ?? "",
                        CreateDate = user.CreateDate,
                    };
        // query = ApplyOrdering(query, queryInfo.OrderBy, queryInfo.OrderType.ToString());
        var entities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();


        return new QueryResult<User>
        {
            Data = _mapper.Map<List<User>>(entities),
            TotalCount = queryInfo.NeedTotalCount
                ? await query.CountAsync()
                : 0
        };
    }

    public async Task<UserForTokenModel?> GetUserPermissionsAsync(Guid id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserForTokenModel
            {
                Roles = u.UserRoles.Select(ur => new Role { Name = ur.Role.Name }).ToList(),
                Permissions = u.UserRoles
                                .SelectMany(ur => ur.Role.RolePermissions)
                                .Select(rp => new Permission
                                {
                                    Id = rp.Permission!.Id,
                                    Resource = rp.Permission.Resource,
                                    Action = rp.Permission.Action
                                })
                                .Distinct()
                                .ToList()
            })
            .FirstOrDefaultAsync();

        if (user == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), id));

        return user;
    }

    public async Task<User> GetByRefreshTokenAsync(string refreshToken)
    {
        var entities = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenExpiryTime > DateTime.Now);

        return _mapper.Map<User>(entities);
    }

    public async Task<UserForTokenModel> GetByIdForRefreshTokenAsync(Guid id)
    {
        var user = await (from u in _context.Users.AsNoTracking()
                          where u.Id == id
                          select new UserForTokenModel
                          {
                              Id = u.Id,
                              StoreId = u.StoreId ?? Guid.Empty,
                              Email = u.Email,
                              Name = u.Name,
                              Picture = u.Picture,
                              IsDeleted = u.IsDeleted,
                              RefreshToken = u.RefreshToken,
                              RefreshTokenExpiryTime = u.RefreshTokenExpiryTime
                          }).FirstOrDefaultAsync();

        return user ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), id));
    }
    public async Task<UserModel> GetUserByIdAsync(Guid id)
    {
        var result = await (from user in _context.Users.AsNoTracking()
                            where user.Id == id
                            join store in _context.Stores.AsNoTracking() on user.StoreId equals store.Id into storeGroup
                            from store in storeGroup.DefaultIfEmpty()
                            join role in _context.UserRoles.AsNoTracking() on user.Id equals role.UserId into roleGroup
                            from role in roleGroup.DefaultIfEmpty()
                            select new UserModel
                            {
                                Id = user.Id,
                                Name = user.Name,
                                Email = user.Email,
                                Picture = user.Picture,
                                Gender = user.Gender,
                                Phone = user.Phone,
                                DOB = user.DOB.ToString("yyyy-MM-dd"),
                                Address = user.Address,
                                Role = role.Role.Name,
                                StoreName = store.Name,
                            }).FirstOrDefaultAsync();
        return result ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), id));
    }

    private IQueryable<User> ApplyOrdering(IQueryable<User> query, string? orderBy, string? orderType)
    {
        var isDesc = orderType == OrderType.Descending.ToString();

        return orderBy?.ToLower() switch
        {
            "name" => isDesc
                ? query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Name).ThenBy(x => x.Id),

            "email" => isDesc
                ? query.OrderByDescending(x => x.Email).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.Email).ThenBy(x => x.Id),

            "createdate" => isDesc
                ? query.OrderByDescending(x => x.CreateDate).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.CreateDate).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.CreateDate).ThenByDescending(x => x.Id)
        };
    }

    public async Task<List<UserModel>> GetUsersByRoleAsync(string roleName)
    {
        var entities = await _context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .Where(u => !u.IsDeleted && u.UserRoles.Any(ur => ur.Role.Name == roleName))
        .ToListAsync();
        return _mapper.Map<List<UserModel>>(entities);
    }

    public async Task<List<UserEntity>> GetUsersByStoreIdAsync(Guid storeId)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.StoreId == storeId && !u.IsDeleted)
            .ToListAsync();
    }
}