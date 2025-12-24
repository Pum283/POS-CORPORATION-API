namespace Application;

[RegisterService(typeof(IPromotionsService))]
public class PromotionsService
    (IPromotionsRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IPromotionsService
{
    public async Task<QueryResult<Promotion>> GetPromotionsAsync(QueryInfo queryInfo, GetPromotionsQuery promotionsQuery)
    {
        return await _repository.GetPromotionsAsync(queryInfo, promotionsQuery);
    }
    public async Task<Promotion> GetPromotionByCodeAsync(string code)
    {
        var result = await _unitOfWork.Repository<PromotionEntity>().GetByConditionAsync(x => x.Code == code);
        if (result == null)
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Promotion), code));
        }
        return _mapper.Map<Promotion>(result);
    }

    public async Task<Promotion> GetPromotionByIdAsync(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        return _mapper.Map<Promotion>(result);
    }

    public async Task<List<PromotionOfProductModel>> GetPromotionsOfProductWithPagingAsync(Guid productId, Guid storeId)
    {
        if (!await _unitOfWork.Repository<ProductEntity>().CheckExistByIdAsync(productId))
        {
            throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ProductEntity)));
        }
        return await _repository.GetPromotionsOfProductWithPagingAsync(productId, storeId);
    }

    public async Task<PromotionModel> GetPromotionByProductIdAsync(Guid productId, Guid storeId)
    {
        return await _repository.GetPromotionByProductIdAsync(productId, storeId);
    }

    public async Task<PromotionModel> GetPromotionByProductGroupIdAsync(Guid productId, Guid productGroupId, Guid storeId)
    {
        return await _repository.GetPromotionByProductGroupIdAsync(productId, productGroupId, storeId);
    }

    public async Task<OperationResult> CreateAsync(CreatePromotionStoreModel model)
    {
        await CheckIdExist(model);

        var promotionEntity = _mapper.Map<PromotionEntity>(model.CreatePromotionModel);
        promotionEntity.Id = Guid.NewGuid();
        promotionEntity.Code = TextHelper.GenerateCodePromotion(10);

        await _unitOfWork.Repository<PromotionEntity>().AddAsync(promotionEntity);
        await CreatePromotionByIdExist(model, promotionEntity);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Promotion)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Promotion)));
    }

    public async Task<OperationResult> UpdateAsync(Guid id, UpdatePromotionModel model)
    {
        var entity = await _unitOfWork.Repository<PromotionEntity>().GetByIdAsync(id);
        _mapper.Map(model, entity);
        await _unitOfWork.Repository<PromotionEntity>().UpdateAsync(entity);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(Promotion), id));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(Promotion), id));
    }

    public async Task<OperationResult> DeleteAsync(Guid id)
    {
        await _unitOfWork.Repository<PromotionEntity>().DeleteByIdAsync(id);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(Promotion), id));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(Promotion), id));
    }

    public async Task<OperationResult> BlockOrUnblockAsync(Guid id)
    {
        var entity = await _unitOfWork.Repository<PromotionEntity>().GetByIdAsync(id);
        entity.IsActive = !entity.IsActive;
        await _unitOfWork.Repository<PromotionEntity>().UpdateAsync(entity);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(Promotion), id));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(Promotion), id));
    }
    private async Task CheckIdExist(CreatePromotionStoreModel model)
    {
        if (model.StoreId.HasValue && !await _unitOfWork.Repository<StoreEntity>().CheckExistByIdAsync(model.StoreId.Value))
        {
            throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(StoreEntity)));
        }

        if (model.ProductId.HasValue && !await _unitOfWork.Repository<ProductEntity>().CheckExistByIdAsync(model.ProductId.Value))
        {
            throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ProductEntity)));
        }

        if (model.ProductGroupId.HasValue && !await _unitOfWork.Repository<ProductGroupEntity>().CheckExistByIdAsync(model.ProductGroupId.Value))
        {
            throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ProductGroupEntity)));
        }
    }

    private async Task CreatePromotionByIdExist(CreatePromotionStoreModel model, PromotionEntity promotionEntity)
    {
        if (model.StoreId.HasValue)
        {
            var promotionStoreEntity = new PromotionStoreEntity
            {
                Id = Guid.NewGuid(),
                StoreId = model.StoreId.Value,
                PromotionId = promotionEntity.Id,
            };
            await _unitOfWork.Repository<PromotionStoreEntity>().AddAsync(promotionStoreEntity);
        }

        if (model.ProductId.HasValue)
        {
            var promotionProductEntity = new PromotionProductEntity
            {
                Id = Guid.NewGuid(),
                ProductId = model.ProductId.Value,
                PromotionId = promotionEntity.Id,
            };
            await _unitOfWork.Repository<PromotionProductEntity>().AddAsync(promotionProductEntity);
        }

        if (model.ProductGroupId.HasValue)
        {
            var promotionProductGroupEntity = new PromotionProductGroupEntity
            {
                Id = Guid.NewGuid(),
                ProductGroupId = model.ProductGroupId.Value,
                PromotionId = promotionEntity.Id,
            };
            await _unitOfWork.Repository<PromotionProductGroupEntity>().AddAsync(promotionProductGroupEntity);
        }
    }
}
