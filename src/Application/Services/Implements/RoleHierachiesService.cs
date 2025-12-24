namespace Application;

[RegisterService(typeof(IRoleHierachiesService))]
public class RoleHierachiesService(
    IRoleHierachiesRepository _repository,
    IRolesRepository _rolesRepository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IRoleHierachiesService
{
    public async Task<List<RoleLevelResponse>> GetRoleTreeByLevelAsync()
    {
        return await _repository.GetRoleTreeByLevelAsync();
    }
    public async Task<OperationResult> AddRoleHierarchieAsync(AddOrUpdateRoleHierarchieModel model)
    {
        //Chặn gán chính mình làm cha
        //Chặn tạo vòng lặp phân cấp
        //Chặn gán quan hệ đã tồn tại
        //Gán mới 1 quan hệ ParentRoleId → ChildRoleId
        var ancestorIds = await _rolesRepository.GetAllAncestorsAsync(model.ParentRoleId);
        if (ancestorIds.Contains(model.ChildRoleId))
        {
            return OperationResult.FailureResult("Không thể tạo vòng lặp quan hệ cha–con trong vai trò.");
        }

        var exists = await _unitOfWork.Repository<RoleHierarchyEntity>()
            .CheckExistAsync(h => h.ParentRoleId == model.ParentRoleId && h.ChildRoleId == model.ChildRoleId);

        if (exists)
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(RoleHierarchyEntity)));
        }

        var entity = _mapper.Map<RoleHierarchyEntity>(model);

        await _unitOfWork.Repository<RoleHierarchyEntity>().AddAsync(entity);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(RoleHierarchyEntity)));
        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(RoleHierarchyEntity)));
    }

    public async Task<OperationResult> UpdateRoleHierarchieAsync(Guid id, AddOrUpdateRoleHierarchieModel model)
    {
        var repository = _unitOfWork.Repository<RoleHierarchyEntity>();

        // Tìm quan hệ hiện tại
        var currentEntity = await repository.GetByIdAsync(id);
        if (currentEntity == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(RoleHierarchyEntity), id));

        // Ngăn quan hệ self-loop
        if (model.ParentRoleId == model.ChildRoleId)
            return OperationResult.FailureResult("Không thể thiết lập vai trò làm cha của chính nó.");

        // Ngăn vòng lặp phân cấp
        if (await CausesCycleAsync(model.ParentRoleId, model.ChildRoleId))
            return OperationResult.FailureResult("Cập nhật này sẽ tạo vòng lặp trong cây phân quyền. Vui lòng chọn vai trò khác.");

        // Kiểm tra duplicate
        var isDuplicate = await repository.CheckExistAsync(h =>
            h.ParentRoleId == model.ParentRoleId &&
            h.ChildRoleId == model.ChildRoleId &&
            h.Id != id);
        if (isDuplicate)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.AlreadyExists, nameof(RoleHierarchyEntity)));

        // Cập nhật
        currentEntity.ParentRoleId = model.ParentRoleId;
        currentEntity.ChildRoleId = model.ChildRoleId;
        await repository.UpdateAsync(currentEntity);

        var result = await _unitOfWork.SaveChangesAsync();
        return result
            ? OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(RoleHierarchyEntity)))
            : OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(RoleHierarchyEntity)));
    }
    public async Task<OperationResult> DeleteRoleHierarchyAsync(Guid id)
    {
        await _unitOfWork.Repository<RoleHierarchyEntity>().DeleteByIdAsync(id);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(RoleHierarchyEntity)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(RoleHierarchyEntity)));
    }

    private async Task<bool> CausesCycleAsync(Guid newParentId, Guid newChildId)
    {
        if (newParentId == newChildId)
            return true;

        var visited = new HashSet<Guid>();
        var stack = new Stack<Guid>();
        stack.Push(newChildId); // Duyệt từ con đi xuống

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (!visited.Add(current)) continue;

            var children = await _unitOfWork.Repository<RoleHierarchyEntity>()
                .GetListByConditionAsync(h => h.ParentRoleId == current);

            foreach (var child in children)
            {
                if (child.ChildRoleId == newParentId)
                    return true; // Tạo vòng lặp
                stack.Push(child.ChildRoleId);
            }
        }

        return false; // Không có vòng lặp
    }

  
}
