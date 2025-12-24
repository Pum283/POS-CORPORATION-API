namespace Application;

[RegisterService(typeof(IProductIngredientsService))]
public class ProductIngredientsService(
    IProductIngredientsRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IProductIngredientsService
{
    public async Task<QueryResult<ProductIngredient>> GetIngredientsForProductAsync(Guid productId, QueryInfo queryInfo)
    {
        return await _repository.GetIngredientsForProductAsync(productId, queryInfo);
    }
    public async Task<OperationResult> AddIngredientToProductAsync(AddIngredientForProductModel model)
    {
        if (await _unitOfWork.Repository<ProductIngredientEntity>().CheckExistAsync(x => x.ProductId == model.ProductId
                                                                                         && x.IngredientId == model.IngredientId))
            throw new BadRequestException(string.Format(ErrorMessageBase.AlreadyExists, nameof(ProductIngredient), model.IngredientId));

        var productIngredient = _mapper.Map<ProductIngredientEntity>(model);
        var result = await _unitOfWork.Repository<ProductIngredientEntity>().AddAsync(productIngredient);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(ProductIngredient)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(ProductIngredient)));
    }

    public async Task<OperationResult> AddIngredientsToProductAsync(List<AddIngredientForProductModel> model)
    {
        var inputKeys = model
            .Select(x => $"{x.ProductId}|{x.IngredientId}")
            .ToHashSet();

        var existingKeys = await _unitOfWork.Repository<ProductIngredientEntity>()
            .GetQueryable()
            .Select(x => new
            {
                Key = $"{x.ProductId}|{x.IngredientId}"
            })
            .Select(x => x.Key)
            .ToListAsync();

        var existingKeySet = existingKeys.ToHashSet();

        var validItems = model
            .Where(x => !existingKeySet.Contains($"{x.ProductId}|{x.IngredientId}"))
            .ToList();

        var duplicatedItems = model
            .Where(x => existingKeySet.Contains($"{x.ProductId}|{x.IngredientId}"))
            .ToList();

        if (!validItems.Any())
        {
            return OperationResult.FailureResult("All ingredient-product pairs already exist.");
        }

        var entitiesToInsert = _mapper.Map<List<ProductIngredientEntity>>(validItems);
        var insertResult = await _unitOfWork.Repository<ProductIngredientEntity>().AddRangeAsync(entitiesToInsert);
        var saveResult = await _unitOfWork.SaveChangesAsync();

        if (!insertResult || !saveResult)
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(ProductIngredient)));
        }

        return OperationResult.SuccessResult(
            ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(ProductIngredient)),
            new
            {
                InsertedCount = validItems.Count,
                DuplicatedCount = duplicatedItems.Count
            });
    }

    public async Task<OperationResult> UpdateIngredientInProductAsync(Guid id, UpdateProductIngredientModel model)
    {
        var productIngredient = await _unitOfWork.Repository<ProductIngredientEntity>().GetByConditionAsync(x => x.Id == id);
        if (productIngredient == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ProductIngredient), id));

        productIngredient.Quantity = model.Quantity;
        _unitOfWork.Repository<ProductIngredientEntity>().UpdateWithoutSave(productIngredient);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(ProductIngredient)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(ProductIngredient)));
    }
    #region DeductIngredient cũ
    //public async Task<OperationResult> DeductIngredientAsync(List<DeductIngredientModel> models)
    //{
    //    var productIds = models.Select(m => m.ProductId).Distinct().ToList();

    //    var productIngredients = await _unitOfWork.Repository<ProductIngredientEntity>()
    //        .GetListByConditionAsync(x => productIds.Contains(x.ProductId));

    //    var ingredientDeductionMap = new Dictionary<Guid, decimal>();
    //    foreach (var model in models)
    //    {
    //        var ingredients = productIngredients.Where(pi => pi.ProductId == model.ProductId);
    //        foreach (var ingredient in ingredients)
    //        {
    //            if (ingredientDeductionMap.ContainsKey(ingredient.IngredientId))
    //                ingredientDeductionMap[ingredient.IngredientId] += ingredient.Quantity * model.Quantity;
    //            else
    //                ingredientDeductionMap[ingredient.IngredientId] = ingredient.Quantity * model.Quantity;
    //        }
    //    }

    //    var ingredientIds = ingredientDeductionMap.Keys.ToList();
    //    var ingredientInventories = await _unitOfWork.Repository<IngredientInventoryEntity>()
    //        .GetListByConditionAsync(x => ingredientIds.Contains(x.IngredientId));

    //    foreach (var inventory in ingredientInventories)
    //    {
    //        if (ingredientDeductionMap.TryGetValue(inventory.IngredientId, out var quantityToDeduct))
    //        {
    //            inventory.Quantity -= quantityToDeduct;
    //            if (inventory.Quantity < 0) inventory.Quantity = 0;

    //            _unitOfWork.Repository<IngredientInventoryEntity>().UpdateWithoutSave(inventory);
    //        }
    //    }

    //    await _unitOfWork.SaveChangesAsync();

    //    return OperationResult.SuccessResult(
    //        ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(ProductIngredient)));
    //}
    #endregion
    public async Task DeductIngredientAsync(List<DeductIngredientModel> models)
    {
        var storeId = models.First().StoreId;

        var productIds = models.Select(m => m.ProductId).Distinct().ToList();

        // Định lượng sản phẩm KHÔNG phụ thuộc chi nhánh
        var productIngredients = await _unitOfWork.Repository<ProductIngredientEntity>()
            .GetListByConditionAsync(x => productIds.Contains(x.ProductId));

        var ingredientDeductionMap = new Dictionary<Guid, decimal>();

        foreach (var model in models)
        {
            var ingredients = productIngredients.Where(pi => pi.ProductId == model.ProductId);

            foreach (var ingredient in ingredients)
            {
                var amount = ingredient.Quantity * model.Quantity;

                if (ingredientDeductionMap.ContainsKey(ingredient.IngredientId))
                    ingredientDeductionMap[ingredient.IngredientId] += amount;
                else
                    ingredientDeductionMap[ingredient.IngredientId] = amount;
            }
        }

        var ingredientIds = ingredientDeductionMap.Keys.ToList();

        // 🔥 FIX CHÍNH — Tồn kho nguyên liệu PHỤ THUỘC Store
        var ingredientInventories = await _unitOfWork.Repository<IngredientInventoryEntity>()
            .GetListByConditionAsync(x =>
                x.StoreId == storeId &&
                ingredientIds.Contains(x.IngredientId)
            );

        foreach (var inventory in ingredientInventories)
        {
            if (ingredientDeductionMap.TryGetValue(inventory.IngredientId, out var qtyToDeduct))
            {
                // ❌ BỎ ĐI: kiểm tra thiếu kho
                // if (inventory.Quantity < qtyToDeduct)
                //     throw new Exception($"Not enough inventory for ingredient {inventory.IngredientId}");

                // ✔ Cho phép âm kho
                inventory.Quantity -= qtyToDeduct;

                _unitOfWork.Repository<IngredientInventoryEntity>().UpdateWithoutSave(inventory);
            }
        }

        // ❗ Quan trọng: KHÔNG SaveChanges trong service này
    }

    public async Task<OperationResult> DeleteIngredientFromProductAsync(Guid id)
    {
        var result = await _unitOfWork.Repository<ProductIngredientEntity>().DeleteByIdAsync(id);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(ProductIngredient)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(ProductIngredient)));
    }

}
