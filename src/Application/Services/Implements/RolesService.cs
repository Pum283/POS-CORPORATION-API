namespace Application;

[RegisterService(typeof(IRolesService))]
public class RolesService(
    IRolesRepository _repository,
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IRolesService
{
    public async Task<List<Role>> GetRolesAsync()
    {
        var roles = await _unitOfWork.Repository<RoleEntity>().GetAllAsync();
        return _mapper.Map<List<Role>>(roles);
    }
    public async Task<UserForTokenModel?> GetUserPermissionsAsync(Guid id)
    {
        return await _usersRepository.GetUserPermissionsAsync(id);
    }
    public async Task<List<Permission>> GetPermissionsForUserAsync(Guid userId)
    {
        return await _repository.GetPermissionsForUserAsync(userId);
    }
    public async Task<List<Role>> GetRoleForUserAsync(Guid userId)
    {
        return await _repository.GetRoleForUserAsync(userId);
    }

    public async Task<QueryResult<GroupedPermissionModel>> GetPermissionsForRoleAsync(QueryInfo queryInfo, Guid roleId)
    {
        return await _repository.GetPermissionsForRoleAsync(queryInfo, roleId);
    }
    public async Task<QueryResult<GroupedPermissionModel>> GetPermissionsAsync(QueryInfo queryInfo)
    {
        return await _repository.GetPermissionsAsync(queryInfo);
    }


    public async Task<OperationResult> CreateRoleAsync(CreateOrUpdateRoleModel model)
    {
        var roleRepo = _unitOfWork.Repository<RoleEntity>();
        if (await roleRepo.CheckExistAsync(x => x.Name == model.RoleName))
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, "Role name", model.RoleName));

        var result = await roleRepo.AddAsync(new RoleEntity
        {
            Id = Guid.NewGuid(),
            Name = model.RoleName
        });
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Role)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Role)), result);
    }
    public async Task<OperationResult> UpdateRoleAsync(Guid roleId, CreateOrUpdateRoleModel model)
    {
        var roleRepo = _unitOfWork.Repository<RoleEntity>();
        var role = await roleRepo.GetByIdAsync(roleId);
        if (role == null)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Role)));
        if (await roleRepo.CheckExistAsync(x => x.Name == model.RoleName && x.Id != roleId))
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, "Role name", model.RoleName));

        role.Name = model.RoleName;
        roleRepo.UpdateWithoutSave(role);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(Role)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(Role)));
    }
    public async Task<OperationResult> DeleteRoleAsync(Guid roleId)
    {
        var roleRepo = _unitOfWork.Repository<RoleEntity>();
        var role = await roleRepo.GetByIdAsync(roleId);
        if (role == null)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Role)));

        await roleRepo.DeleteByIdAsync(roleId);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(Role)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(Role)));
    }

    public async Task<OperationResult> DeletePermisionForRoleAsync(Guid roleId)
    {
        var rolePermissionRepo = _unitOfWork.Repository<RolePermissionEntity>();
        var rolePermission = await rolePermissionRepo.GetByConditionAsync(x => x.PermissionId == roleId);
        if (rolePermission == null)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(RolePermissionEntity)));

        await rolePermissionRepo.DeleteByIdAsync(rolePermission.Id);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(RolePermissionEntity)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(RolePermissionEntity)));
    }

    public async Task<OperationResult> AddPermissionsForRole(AddPermissionsForRoleModel model)
    {
        var ancestorRoleIds = await _repository.GetAllAncestorRoleIdsAsync(model.RoleId);

        var ancestorPermissions = await _repository
            .GetPermissionIdsByRolesAsync(ancestorRoleIds);

        var currentPermissions = await _repository
            .GetPermissionIdsByRoleAsync(model.RoleId);

        //  Gộp tất cả quyền bị cấm gán
        var allDeniedPermissions = ancestorPermissions.Union(currentPermissions).ToHashSet();

        //  Kiểm tra quyền bị trùng
        var invalidPermissions = model.PermissionId
            .Where(p => allDeniedPermissions.Contains(p))
            .ToList();

        if (invalidPermissions.Any())
        {
            return OperationResult.FailureResult(
                "Không được gán quyền đã thuộc về chính role hoặc role cha.");
        }

        var rolePermission = _mapper.Map<List<RolePermissionEntity>>(model);
        await _unitOfWork.Repository<RolePermissionEntity>().AddRangeAsync(rolePermission);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(RolePermission)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(RolePermission)));
    }

    public async Task<OperationResult> AddPermissionsForRoles(List<AddPermissionsForRoleModel> models)
    {
        var allRoleIds = models.Select(m => m.RoleId).Distinct().ToList();

        // 1. Get ancestor roles for each role
        var ancestorMap = await _repository.GetAllAncestorsForRolesAsync(allRoleIds); // Dictionary<RoleId, List<AncestorIds>>

        // 2. Gather all involved roles: self + ancestors
        var allRelatedRoleIds = allRoleIds
            .Concat(ancestorMap.SelectMany(kv => kv.Value))
            .Distinct()
            .ToList();

        // 3. Fetch permissions for all roles in one query
        var permissionMap = await _repository.GetPermissionsForRolesMappedAsync(allRelatedRoleIds); // Dictionary<RoleId, HashSet<PermissionId>>

        var allNewRolePermissions = new List<RolePermissionEntity>();

        foreach (var model in models)
        {
            var currentRoleId = model.RoleId;
            var requestedPermissions = model.PermissionId.ToHashSet();

            // Get ancestor permissions
            var ancestorIds = ancestorMap.GetValueOrDefault(currentRoleId) ?? new List<Guid>();
            var ancestorPermissions = ancestorIds
                .SelectMany(id => permissionMap.GetValueOrDefault(id) ?? new HashSet<Guid>())
                .ToHashSet();

            // Get current role permissions
            var currentPermissions = permissionMap.GetValueOrDefault(currentRoleId) ?? new HashSet<Guid>();

            // Combine forbidden permissions
            var allDenied = ancestorPermissions.Union(currentPermissions);

            // Validate
            var invalid = requestedPermissions.Intersect(allDenied).ToList();
            if (invalid.Any())
            {
                return OperationResult.FailureResult($"Role {currentRoleId} đang có quyền bị trùng hoặc thừa kế từ cha.");
            }

            // Map and collect
            var mapped = _mapper.Map<List<RolePermissionEntity>>(model);
            allNewRolePermissions.AddRange(mapped);
        }

        // Save all at once
        await _unitOfWork.Repository<RolePermissionEntity>().AddRangeAsync(allNewRolePermissions);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult("Thêm quyền thất bại.");
        }

        return OperationResult.SuccessResult("Thêm quyền thành công.");
    }

}