namespace Application;

[RegisterService(typeof(IPromotionStoresService))]
public class PromotionStoresService(
    IPromotionStoresRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IPromotionStoresService
{
    public async Task<PromotionStoreModel> GetPromotionsOfStoreWithPagingAsync(Guid storeId, QueryInfo queryInfo)
    {
        if (!await _unitOfWork.Repository<StoreEntity>().CheckExistByIdAsync(storeId))
        {
            throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(StoreEntity), storeId));
        }
        return await _repository.GetPromotionsOfStorePagingAsync(storeId, queryInfo);
    }

    public Task<PromotionStoreModel> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public async Task<OperationResult> UpdateAsync(Guid promotionId, UpdatePromotionStoreModel model)
    {
        var entity = await _unitOfWork.Repository<PromotionStoreEntity>().GetByConditionAsync(x => x.StoreId == model.OldStoreId && x.PromotionId == promotionId);
        if (entity == null)
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(PromotionStoreModel), promotionId));
        }
        _mapper.Map(model, entity);
        await _unitOfWork.Repository<PromotionStoreEntity>().UpdateAsync(entity);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(PromotionStoreEntity)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(PromotionStoreEntity)));
    }

    public Task<OperationResult> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
