namespace Application;

[RegisterService(typeof(IStoresService))]
public class StoresService(
    IStoresRepository _repository,
    IProductsRepository _productsRepository,
    IIngredientsRepository _ingredientsRepository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IStoresService
{
    public async Task<QueryResult<Store>> GetStoresWithPagingAsync(QueryInfo queryInfo)
    {
        return await _repository.GetStoresWithPagingAsync(queryInfo);
    }

    public async Task<QueryResult<Store>> GetStoresWithoutAccountAsync(QueryInfo queryInfo)
    {
        return await _repository.GetStoresWithoutAccountAsync(queryInfo);
    }
    public async Task<Store> GetStoreByIdAsync(Guid id)
    {
        var storeEntity = await _unitOfWork.Repository<StoreEntity>().GetByIdAsync(id);
        return _mapper.Map<Store>(storeEntity);
    }

    public async Task<List<Guid>> GetStoresByProductIdAsync(Guid id)
    {
        return await _repository.GetStoresByProductIdAsync(id);
    }

    public async Task<OperationResult> CreateStoreAsync(CreateOrUpdateStoreModel model)
    {
        var checkExistingStore = await _unitOfWork.Repository<StoreEntity>()
            .CheckExistAsync(x => x.Address.Trim().ToLower() == model.Address!.Trim().ToLower());

        if (checkExistingStore)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.Existed, nameof(ProductGroup), model.Address!));

        var storeEntity = _mapper.Map<StoreEntity>(model);
        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                // Add store
                await _unitOfWork.Repository<StoreEntity>().AddAsync(storeEntity);

                // Get product ids
                var productIds = await _productsRepository.GetProductIdsAsync();
                var ingredientIds = await _ingredientsRepository.GetIngredientIdsAsync();

                var inventoryEntitys = productIds.Select(productId => new InventoryEntity
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    StoreId = storeEntity.Id,
                    Quantity = 0
                }).ToList();

                var ingredientInventoryEntitys = ingredientIds.Select(ingredientId => new IngredientInventoryEntity
                {
                    Id = Guid.NewGuid(),
                    IngredientId = ingredientId,
                    StoreId = storeEntity.Id,
                    Quantity = 0
                }).ToList();

                await _unitOfWork.Repository<InventoryEntity>().AddRangeAsync(inventoryEntitys);
                await _unitOfWork.Repository<IngredientInventoryEntity>().AddRangeAsync(ingredientInventoryEntitys);
            });
            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(StoreEntity)));
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(StoreEntity)) + $". {ex.Message}");
        }
    }
    public async Task<OperationResult> CreateStoresAsync(List<CreateOrUpdateStoreModel> model)
    {
        var inputAddresses = model
            .Select(x => x.Address!.Trim().ToLower())
            .Distinct()
            .ToList();

        var existedAddresses = await _unitOfWork.Repository<StoreEntity>()
            .GetQueryable()
            .Where(x => inputAddresses.Contains(x.Address.Trim().ToLower()))
            .Select(x => x.Address.Trim().ToLower())
            .ToListAsync();

        var validStores = model
            .Where(x => !existedAddresses.Contains(x.Address!.Trim().ToLower()))
            .ToList();

        var duplicateStores = model
            .Where(x => existedAddresses.Contains(x.Address!.Trim().ToLower()))
            .ToList();

        if (validStores.Any())
        {
            var storesToInsert = _mapper.Map<List<StoreEntity>>(validStores);
            var insertResult = await _unitOfWork.Repository<StoreEntity>().AddRangeAsync(storesToInsert);
            await _unitOfWork.SaveChangesAsync();

            if (!insertResult)
            {
                return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(StoreEntity)));
            }
        }

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(StoreEntity)),
            new
            {
                InsertedCount = validStores.Count,
                DuplicatedCount = duplicateStores.Count,
                Duplicates = duplicateStores
            }
        );
    }

    public async Task<OperationResult> UpdateStoreAsync(Guid id, CreateOrUpdateStoreModel model)
    {
        var checkExistingStore = await _unitOfWork.Repository<StoreEntity>()
            .CheckExistAsync(x => x.Address.ToLower() == model.Address!.ToLower() && x.Id != id);

        if (checkExistingStore)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.Existed, nameof(ProductGroup), model.Name!));

        var storeEntity = _mapper.Map<StoreEntity>(model);
        storeEntity.Id = id;
        var result = await _unitOfWork.Repository<StoreEntity>().UpdateAsync(storeEntity);
        await _unitOfWork.SaveChangesAsync();
        if (!result)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(StoreEntity)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(StoreEntity)));
    }
    public async Task<OperationResult> DeleteStoreAsync(Guid id)
    {
        var result = await _unitOfWork.Repository<StoreEntity>().DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
        if (result == 0)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(StoreEntity)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(StoreEntity)));
    }
}
