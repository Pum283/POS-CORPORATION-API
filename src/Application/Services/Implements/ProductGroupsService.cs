namespace Application;

[RegisterService(typeof(IProductGroupsService))]
public class ProductGroupsService(
    IProductGroupsRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IProductGroupsService
{
    public Task<QueryResult<ProductGroup>> GetProductGroupsWithPaging(QueryInfo queryInfo)
    {
        return _repository.GetProductGroupsWithPaging(queryInfo);
    }
    public async Task<QueryResult<Product>> GetProductsByGroupIdWithPagingAsync(Guid storeId, Guid productGroupId, QueryInfo queryInfo)
    {
        return await _repository.GetProductsByGroupIdWithPagingAsync(storeId, productGroupId, queryInfo);
    }
    public async Task<QueryResult<ProductGroup>> GetProductGroupByCategoryIdWithPaging(Guid categoryId, QueryInfo queryInfo)
    {
        return await _repository.GetProductGroupByCategoryIdWithPaging(categoryId, queryInfo);
    }

    public async Task<ProductGroup> GetByIdAsync(Guid id)
    {
        var result = await _unitOfWork.Repository<ProductGroupEntity>().GetByIdAsync(id);
        return _mapper.Map<ProductGroup>(result);
    }

    public async Task<OperationResult> CreateAsync(CreateOrUpdateProductGroupModel model)
    {
        var checkExistingGroup = await _unitOfWork.Repository<ProductGroupEntity>()
            .CheckExistAsync (x => x.Name == model.Name!);

        if (checkExistingGroup)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.Existed, nameof(ProductGroup), model.Name!));

        var productGroup = _mapper.Map<ProductGroupEntity>(model);
        var result = await _unitOfWork.Repository<ProductGroupEntity>().AddAsync(productGroup);
        await _unitOfWork.SaveChangesAsync();
        if (result == null)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(ProductGroup)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(ProductGroup)), model);
    }

    public async Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateProductGroupModel model)
    {
        var productGroupEntity = await _unitOfWork.Repository<ProductGroupEntity>().GetByIdAsync(id);

        if(productGroupEntity.Name != model.Name)
        {
            var checkExistingProductGroup = await _unitOfWork.Repository<ProductGroupEntity>()
            .CheckExistAsync(x => x.Name == model.Name!);

            if (checkExistingProductGroup)
                return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.Existed, nameof(ProductGroup), model.Name!));
        }

        var productGroup = _mapper.Map(model, productGroupEntity);
        var result = await _unitOfWork.Repository<ProductGroupEntity>().UpdateAsync(productGroupEntity);
        await _unitOfWork.SaveChangesAsync();
        if (!result)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(ProductGroup)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(ProductGroup), model));
    }

    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        var result = await _unitOfWork.Repository<ProductGroupEntity>().DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
        if (result == 0)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(ProductGroup)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(ProductGroup)));
    }
}
