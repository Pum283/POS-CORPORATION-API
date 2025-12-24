namespace Application;

[RegisterService(typeof(IIngredientsService))]
public class IngredientsService(
    IIngredientsRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IIngredientsService
{
    public async Task<QueryResult<Ingredient>> GetIngredientsWithPagingAsync(QueryInfo queryInfo)
    {
       return await _repository.GetIngredientsWithPagingAsync(queryInfo);
    }
    public async Task<QueryResult<IngredientsInventoryReportModel>> GetStoreIngredientsPagedAsync(QueryInfo queryInfo, Guid storeId, DateTime? date = null)
    {
        return await _repository.GetStoreIngredientsPagedAsync(queryInfo, storeId);
    }

    public async Task<HashSet<string>> GetCodesAsync()
    {
        return await _repository.GetIngredientCodesAsync();
    }
    public async Task<Ingredient> GetIngredientByIdAsync(Guid id)
    {
        var ingredientEntity = await _unitOfWork.Repository<IngredientEntity>().GetByIdAsync(id);
        return _mapper.Map<Ingredient>(ingredientEntity);
    }

    public async Task<Ingredient> GetIngredientByCodeAsync(string code)
    {
        var ingredientEntity = await _unitOfWork.Repository<IngredientEntity>().GetByConditionAsync(x => x.Code.Trim().ToLower() == code.Trim().ToLower());
        return _mapper.Map<Ingredient>(ingredientEntity);
    }

    public async Task<QueryResult<LowStockModel>> GetLowStockIngredientsAsync(Guid storeId, QueryInfo queryInfo)
        => await _repository.GetLowStockIngredientsAsync(storeId, queryInfo);
    #region phương thức create cũ
    //public async Task<OperationResult> CreateIngredientAsync(CreateOrUpdateIngredientModel model)
    //{
    //    if (await _unitOfWork.Repository<IngredientEntity>().CheckExistAsync(x => x.Code == model.Code))
    //    {
    //        throw new BadRequestException(string.Format(ErrorMessageBase.AlreadyExists, nameof(Ingredient), model.Code));
    //    }

    //    var ingredient = _mapper.Map<IngredientEntity>(model);
    //    var result = await _unitOfWork.Repository<IngredientEntity>().AddAsync(ingredient);
    //    await _unitOfWork.SaveChangesAsync();
    //    if (result == null)
    //        return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Ingredient)));
    //    return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Ingredient)));
    //}

    //public async Task<OperationResult> CreateIngredientsAsync(List<CreateOrUpdateIngredientModel> model)
    //{
    //    var inputCodes = model
    //        .Select(x => x.Code!.Trim().ToLower())
    //        .Distinct()
    //        .ToList();


    //    var existedCodesAndNames = await _unitOfWork.Repository<IngredientEntity>()
    //        .GetQueryable()
    //        .Where(x => inputCodes.Contains(x.Code.Trim().ToLower()))
    //        .Select(x => new
    //        {
    //            Code = x.Code.Trim().ToLower(),
    //        })
    //        .ToListAsync();

    //    var existedCodes = existedCodesAndNames.Select(x => x.Code).ToHashSet();

    //    var validIngredients = model
    //        .Where(x => !existedCodes.Contains(x.Code!.Trim().ToLower()))
    //        .ToList();

    //    var duplicatedIngredients = model
    //        .Where(x => existedCodes.Contains(x.Code!.Trim().ToLower()))
    //        .ToList();

    //    if (validIngredients.Any())
    //    {
    //        var ingredientsToInsert = _mapper.Map<List<IngredientEntity>>(validIngredients);
    //        var insertResult = await _unitOfWork.Repository<IngredientEntity>().AddRangeAsync(ingredientsToInsert);
    //        var saveResult =  await _unitOfWork.SaveChangesAsync();

    //        if (!insertResult || !saveResult)
    //        {
    //            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Ingredient)));
    //        }
    //    }

    //    if(validIngredients.Count == 0)
    //    {
    //        return OperationResult.SuccessResult(
    //        ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Ingredient)),
    //        new
    //        {
    //            InsertedCount = validIngredients.Count,
    //            DuplicatedCount = duplicatedIngredients.Count,
    //            Duplicates = duplicatedIngredients
    //        }
    //    );
    //    }

    //    return OperationResult.SuccessResult(
    //        ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Ingredient)),
    //        new
    //        {
    //            InsertedCount = validIngredients.Count,
    //            DuplicatedCount = duplicatedIngredients.Count,
    //            Duplicates = duplicatedIngredients
    //        }
    //    );
    //}
    #endregion

    public async Task<OperationResult> CreateIngredientAsync(CreateOrUpdateIngredientModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Code))
            throw new BadRequestException("Mã nhiên liệu không được để trống.");

        var code = model.Code.Trim().ToUpper();

        // Kiểm tra trùng Ingredient
        var existedIngredient = await _unitOfWork.Repository<IngredientEntity>()
            .CheckExistAsync(x => x.Code.Trim().ToUpper() == code);

        // Kiểm tra trùng Product SKU
        var existedProductSku = await _unitOfWork.Repository<ProductEntity>()
            .CheckExistAsync(x => x.SKU.Trim().ToUpper() == code);

        if (existedIngredient)
            throw new BadRequestException($"Mã nhiên liệu đã tồn tại: {model.Code}");

        if (existedProductSku)
            throw new BadRequestException($"Mã nhiên liệu trùng với SKU sản phẩm: {model.Code}");

        var ingredient = _mapper.Map<IngredientEntity>(model);
        var result = await _unitOfWork.Repository<IngredientEntity>().AddAsync(ingredient);
        await _unitOfWork.SaveChangesAsync();

        //Thêm logic tự động thêm vào inven với số lượng 0 khi tạo mới nl
        // TỰ ĐỘNG TẠO TỒN KHO = 0 CHO TẤT CẢ CỬA HÀNG (nếu chưa có)
        var storeIdsWithoutInventory = await _unitOfWork.Repository<StoreEntity>()
            .GetQueryable()
            .Where(s => !_unitOfWork.Repository<IngredientInventoryEntity>()
                .GetQueryable()
                .Any(i => i.IngredientId == ingredient.Id && i.StoreId == s.Id))
            .Select(s => s.Id)
            .ToListAsync();

        if (storeIdsWithoutInventory.Any())
        {
            var inventories = storeIdsWithoutInventory.Select(storeId => new IngredientInventoryEntity
            {
                Id = Guid.NewGuid(),
                IngredientId = ingredient.Id,
                StoreId = storeId,
                Quantity = 0
            }).ToList();

            await _unitOfWork.Repository<IngredientInventoryEntity>().AddRangeAsync(inventories);
            await _unitOfWork.SaveChangesAsync();
        }

        if (result == null)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Ingredient)));

        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Ingredient)));
    }

    //public async Task<OperationResult> CreateIngredientsAsync(List<CreateOrUpdateIngredientModel> model)
    //{
    //    if (model == null || !model.Any())
    //        return OperationResult.FailureResult("Danh sách nhiên liệu trống.");

    //    // Chuẩn hóa code
    //    var inputCodes = model
    //        .Select(x => x.Code!.Trim().ToUpper())
    //        .Distinct()
    //        .ToList();

    //    // 1️⃣ Kiểm tra trùng Ingredient
    //    var existedIngredientCodes = await _unitOfWork.Repository<IngredientEntity>()
    //        .GetQueryable()
    //        .Where(x => inputCodes.Contains(x.Code.Trim().ToUpper()))
    //        .Select(x => x.Code.Trim().ToUpper())
    //        .ToListAsync();

    //    // 2️⃣ Kiểm tra trùng Product SKU
    //    var existedProductSkus = await _unitOfWork.Repository<ProductEntity>()
    //        .GetQueryable()
    //        .Where(x => inputCodes.Contains(x.SKU.Trim().ToUpper()))
    //        .Select(x => x.SKU.Trim().ToUpper())
    //        .ToListAsync();

    //    var existedCodes = existedIngredientCodes
    //        .Union(existedProductSkus)
    //        .ToHashSet();

    //    var validIngredients = model
    //        .Where(x => !existedCodes.Contains(x.Code!.Trim().ToUpper()))
    //        .ToList();

    //    var duplicatedIngredients = model
    //        .Where(x => existedCodes.Contains(x.Code!.Trim().ToUpper()))
    //        .ToList();

    //    // Chèn dữ liệu hợp lệ
    //    if (validIngredients.Any())
    //    {
    //        var ingredientsToInsert = _mapper.Map<List<IngredientEntity>>(validIngredients);
    //        var insertResult = await _unitOfWork.Repository<IngredientEntity>().AddRangeAsync(ingredientsToInsert);
    //        var saveResult = await _unitOfWork.SaveChangesAsync();

    //        if (!insertResult || !saveResult)
    //        {
    //            return OperationResult.FailureResult(
    //                ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Ingredient))
    //            );
    //        }
    //    }

    //    return OperationResult.SuccessResult(
    //        validIngredients.Any()
    //            ? ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Ingredient))
    //            : ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Ingredient)),
    //        new
    //        {
    //            InsertedCount = validIngredients.Count,
    //            DuplicatedCount = duplicatedIngredients.Count,
    //            Duplicates = duplicatedIngredients
    //        }
    //    );
    //}

    public async Task<OperationResult> CreateIngredientsAsync(List<CreateOrUpdateIngredientModel> model)
    {
        if (model == null || !model.Any())
            return OperationResult.FailureResult("Danh sách nhiên liệu trống.");

        // Chuẩn hóa code
        var inputCodes = model
            .Select(x => x.Code!.Trim().ToUpper())
            .Distinct()
            .ToList();

        // --- 1️⃣ Kiểm tra trùng trên cả Ingredient + Product cùng lúc ---
        var existedCodes = await _unitOfWork.Repository<IngredientEntity>()
            .GetQueryable()
            .Where(x => inputCodes.Contains(x.Code.Trim().ToUpper()))
            .Select(x => x.Code.Trim().ToUpper())
            .Union(
                _unitOfWork.Repository<ProductEntity>()
                .GetQueryable()
                .Where(x => inputCodes.Contains(x.SKU.Trim().ToUpper()))
                .Select(x => x.SKU.Trim().ToUpper())
            )
            .ToListAsync();

        if (existedCodes.Any())
        {
            // Nếu có trùng → trả về lỗi liệt kê mã trùng
            var duplicates = model
                .Where(x => existedCodes.Contains(x.Code!.Trim().ToUpper()))
                .Select(x => x.Code)
                .ToList();

            return OperationResult.FailureResult(
                $"Các mã nhiên liệu bị trùng hoặc trùng với SKU sản phẩm: {string.Join(", ", duplicates)}"
            );
        }

        // --- 2️⃣ Thêm dữ liệu hợp lệ ---
        var ingredientsToInsert = _mapper.Map<List<IngredientEntity>>(model);
        var insertResult = await _unitOfWork.Repository<IngredientEntity>().AddRangeAsync(ingredientsToInsert);
        var saveResult = await _unitOfWork.SaveChangesAsync();

        if (insertResult && saveResult)
        {
            // Lấy tất cả StoreId (chỉ query 1 lần)
            var allStoreIds = await _unitOfWork.Repository<StoreEntity>()
                .GetQueryable()
                .Select(s => s.Id)
                .ToListAsync();

            // Kiểm tra những cặp nào đã tồn tại
            var newIngredientIds = ingredientsToInsert.Select(i => i.Id).ToList();
            var existingPairs = await _unitOfWork.Repository<IngredientInventoryEntity>()
                .GetQueryable()
                .Where(i => newIngredientIds.Contains(i.IngredientId))
                .Select(i => new { i.IngredientId, i.StoreId })
                .ToListAsync();

            var existingLookup = existingPairs.ToLookup(x => x.IngredientId, x => x.StoreId);
            var inventoriesToAdd = new List<IngredientInventoryEntity>();

            foreach (var ingredient in ingredientsToInsert)
            {
                var existingStores = existingLookup[ingredient.Id].ToHashSet();
                foreach (var storeId in allStoreIds)
                {
                    if (!existingStores.Contains(storeId))
                    {
                        inventoriesToAdd.Add(new IngredientInventoryEntity
                        {
                            Id = Guid.NewGuid(),
                            IngredientId = ingredient.Id,
                            StoreId = storeId,
                            Quantity = 0
                        });
                    }
                }
            }

            if (inventoriesToAdd.Any())
            {
                await _unitOfWork.Repository<IngredientInventoryEntity>().AddRangeAsync(inventoriesToAdd);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        if (!insertResult || !saveResult)
        {
            return OperationResult.FailureResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Ingredient))
            );
        }

        return OperationResult.SuccessResult(
            ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Ingredient)),
            new
            {
                InsertedCount = model.Count
            }
        );
    }

    public async Task<OperationResult> UpdateIngredientAsync(Guid id, CreateOrUpdateIngredientModel model)
    {
        var ingredientEntity = await _unitOfWork.Repository<IngredientEntity>().GetByIdAsync(id);

        if (ingredientEntity.Code != model.Code)
        {
            if (await _unitOfWork.Repository<IngredientEntity>().CheckExistAsync(x => x.Code == model.Code))
            {
                throw new BadRequestException(string.Format(ErrorMessageBase.AlreadyExists, nameof(User), model.Code));
            }
        }

        if (ingredientEntity.Name != model.Name)
        {
            if (await _unitOfWork.Repository<IngredientEntity>().CheckExistAsync(x => x.Name == model.Name))
            {
                throw new BadRequestException(string.Format(ErrorMessageBase.AlreadyExists, nameof(User), model.Name));
            }
        }

        var ingredient = _mapper.Map(model, ingredientEntity);
        var result = await _unitOfWork.Repository<IngredientEntity>().UpdateAsync(ingredient);
        await _unitOfWork.SaveChangesAsync();
        if (!result)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(Ingredient)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(Ingredient)));
    }

    public async Task<OperationResult> DeleteIngredientAsync(Guid id)
    {
        var result = await _unitOfWork.Repository<IngredientEntity>().DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
        if (result == 0)
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeleteFailure, nameof(Ingredient)));
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(Ingredient)));
    }

    public async Task<(bool IsValid, string? SuggestCode, string? Reason)> CheckIngredientCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new Exception("Mã nhiên liệu không được để trống.");

        var input = code.Trim().ToUpper();

        // Tách prefix và number
        string prefix = input;
        int inputNumber = 0;

        if (input.Contains('-'))
        {
            var parts = input.Split('-');
            prefix = parts[0];

            if (parts.Length > 1 && int.TryParse(parts[1], out var num))
                inputNumber = num;
        }

        // 1️⃣ Kiểm tra trùng Ingredient
        var existedIngredient = await _unitOfWork.Repository<IngredientEntity>()
            .GetQueryable()
            .AnyAsync(x => x.Code.ToLower() == input.ToLower());

        if (!existedIngredient)
        {
            // 2️⃣ Kiểm tra trùng Product SKU
            var existedProductSku = await _unitOfWork.Repository<ProductEntity>()
                .GetQueryable()
                .AnyAsync(x => x.SKU.ToLower() == input.ToLower());

            if (!existedProductSku)
            {
                // Không trùng gì → hợp lệ
                return (true, null, null);
            }
            else
            {
                // Trùng SKU product → đề xuất mã mới
                string suggest = await SuggestNextIngredientCode(prefix, inputNumber);
                return (false, suggest, "Code trùng với SKU sản phẩm.");
            }
        }
        else
        {
            // Trùng Ingredient → đề xuất mã mới
            string suggest = await SuggestNextIngredientCode(prefix, inputNumber);
            return (false, suggest, "Code đã tồn tại trong danh sách nhiên liệu.");
        }
    }
    private async Task<string> SuggestNextIngredientCode(string prefix, int inputNumber)
    {
        var upperPrefix = (prefix + "-").ToUpper();

        // Lấy tất cả mã bắt đầu bằng prefix hoặc chính prefix
        var allCodes = await _unitOfWork.Repository<IngredientEntity>()
            .GetQueryable()
            .Where(x => x.Code.ToUpper() == prefix.ToUpper() || x.Code.ToUpper().StartsWith(upperPrefix))
            .Select(x => x.Code)
            .ToListAsync();

        int maxNumber = 0;

        foreach (var code in allCodes)
        {
            if (code.Contains('-'))
            {
                var parts = code.Split('-');
                if (int.TryParse(parts.Last(), out var num))
                    maxNumber = Math.Max(maxNumber, num);
            }
            else
            {
                // Nếu chỉ là prefix không có số → coi là số 0
                maxNumber = Math.Max(maxNumber, 0);
            }
        }

        int nextNumber = Math.Max(inputNumber + 1, maxNumber + 1);
        return $"{prefix}-{nextNumber:D3}";
    }
}
