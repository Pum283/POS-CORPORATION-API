namespace Application;

[RegisterService(typeof(IInventoriesService))]
public class InventoriesService(
    IInventoriesRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IInventoriesService
{
    public async Task<QueryResult<InventoryViewModel>> GetInventoriesWithPagingAsync(QueryInfo queryInfo)
    {
        return await _repository.GetInventoriesWithPagingAsync(queryInfo);
    }
    public async Task<QueryResult<InventoryViewModel>> GetInventoriesWithPagingByStoreIdAsync(Guid storeId, QueryInfo queryInfo)
    {
        return await _repository.GetInventoriesWithPagingByStoreIdAsync(storeId, queryInfo);
    }
    public async Task<QueryResult<StoreInventoryByProductCodeModel>> GetStoresByProductCodeAsync(string productCode, Guid currentStoreId, QueryInfo queryInfo)
        => await _repository.GetStoresByProductCodeAsync(productCode, currentStoreId, queryInfo);
    public async Task<OperationResult> CreateAsync(CreateOrUpdateInventoryModel model)
    {
        var entity = _mapper.Map<InventoryEntity>(model);
        var result = await _unitOfWork.Repository<InventoryEntity>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        if (result == null)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Inventory)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Inventory), result));
    }
    public async Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateInventoryModel model)
    {
        var entity = await _unitOfWork.Repository<InventoryEntity>().GetByIdAsync(id);
        _mapper.Map(model, entity);
        var result = await _unitOfWork.Repository<InventoryEntity>().UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        if (!result)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(Inventory)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(Inventory)));
    }
}
