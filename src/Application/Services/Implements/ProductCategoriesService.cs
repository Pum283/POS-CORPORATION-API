namespace Application;

[RegisterService(typeof(IProductCategoriesService))]
public class ProductCategoriesService(
    IProductCategoriesRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IProductCategoriesService
{
    public async Task<QueryResult<ProductCategory>> GetProductCategoriesWithPagingAsync(QueryInfo queryInfo)
    {
        return await _repository.GetProductCategoriesWithPagingAsync(queryInfo);
    }
    public async Task<ProductCategory> GetByIdAsync(Guid id)
    {
        var result = await _unitOfWork.Repository<ProductCategoryEntity>().GetByIdAsync(id);
        return _mapper.Map<ProductCategory>(result);
    }
    public async Task<List<ProductCategoriesIncludeProductGroupsModel>> GetProductCategoriesIncludeProductGroupsWithPagingAsync()
        => await _repository.GetProductCategoriesIncludeProductGroupsWithPagingAsync();
    public async Task<OperationResult> CreateAsync(CreateOrUpdateProductCategoryModel model)
    {
        var checkExistingCategory = await _unitOfWork.Repository<ProductCategoryEntity>()
            .CheckExistAsync(x => x.Name.Trim().ToLower() == model.Name!.Trim().ToLower());

        if (checkExistingCategory)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.Existed, nameof(ProductCategory), model.Name!));

        var productCategory = _mapper.Map<ProductCategoryEntity>(model);
        var result = await _unitOfWork.Repository<ProductCategoryEntity>().AddAsync(productCategory);
        await _unitOfWork.SaveChangesAsync();
        if (result == null)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(ProductCategory)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(ProductCategory)), result);
    }

    public async Task<OperationResult> UpdateAsync(Guid id, CreateOrUpdateProductCategoryModel model)
    {
        var productCategory = await _unitOfWork.Repository<ProductCategoryEntity>().GetByIdAsync(id);

        if (productCategory.Name != model.Name)
        {
            var checkExistingGroup = await _unitOfWork.Repository<ProductCategoryEntity>()
            .CheckExistAsync(x => x.Name.ToLower() == model.Name!.ToLower() && x.Id != id);

            if (checkExistingGroup)
                return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.Existed, nameof(ProductGroup), model.Name!));
        }

        _mapper.Map(model, productCategory);
        var result = await _unitOfWork.Repository<ProductCategoryEntity>().UpdateAsync(productCategory);
        await _unitOfWork.SaveChangesAsync();

        if (!result)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(ProductCategory)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(ProductCategory), model));
    }

    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        var result = await _unitOfWork.Repository<ProductCategoryEntity>().DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
        if (result == 0)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(ProductCategory)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(ProductCategory)));
    }
}
