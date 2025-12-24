namespace Application;

[RegisterService(typeof(IProductsService))]
public class ProductsService(
    IProductsRepository _repository,
    IBlobStorageService _blob,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IProductsService
{
    public async Task<QueryResult<Product>> GetAllWithPagingAsync(QueryInfo queryInfo)
    {
        return await _repository.GetAllWithPagingAsync
            (queryInfo);
    }
    public async Task<HashSet<string>> GetSkusAsync()
    {
        return await _repository.GetProductSkusAsync();
    }
    public async Task<QueryResult<ProductWithPromotion>> GetProductsByStoreIdWithPagingAsync(Guid storeId, QueryInfo queryInfo)
    {
        return await _repository.GetProductsByStoreIdWithPaging(storeId, queryInfo);
    }
    public async Task<QueryResult<Product>> GetProductsByCategoryGroupWithPaging(Guid storeId, string categoryGroup, QueryInfo queryInfo)
    {
        return await _repository.GetProductsByCategoryGroupWithPaging(storeId, categoryGroup, queryInfo);
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        return _mapper.Map<Product>(product);
    }
    public async Task<Product> GetBySkuAsync(string sku)
    {
        return await _repository.GetBySkuAsync(sku);
    }
    public async Task<QueryResult<LowStockModel>> GetLowStockProductsAsync(Guid storeId, QueryInfo queryInfo)
        => await _repository.GetLowStockProductsAsync(storeId, queryInfo);
    #region Create cũ 
    //public async Task<OperationResult> CreateAsync(CreateProductModel model)
    //{
    //    string? newPicture = null;
    //    try
    //    {
    //        var productGroupName = await _unitOfWork
    //            .Repository<ProductGroupEntity>()
    //            .GetQueryable()
    //            .Where(x => x.Id == model.ProductGroupId)
    //            .Select(x => x.Name)
    //            .FirstOrDefaultAsync();

    //        if (productGroupName is null)
    //            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ProductGroup), model.ProductGroupId));

    //        newPicture = await _blob.UploadFileAsync(AppConstants.BlobContainer, model.Picture!);

    //        var product = _mapper.Map<ProductEntity>(model);
    //        product.Picture = newPicture;
    //        product.Unit = Enum.TryParse<UnitType>(model.Unit, out var unitType) ? unitType : UnitType.Other;

    //        var prefix = TextHelper.GetPrefix(productGroupName);
    //        var lastSku = await _unitOfWork.Repository<ProductEntity>()
    //            .GetQueryable()
    //            .Where(p => p.ProductGroupId == model.ProductGroupId && p.SKU.StartsWith(prefix))
    //            .OrderByDescending(p => p.SKU)
    //            .Select(p => p.SKU)
    //            .FirstOrDefaultAsync();

    //        int nextNumber = 1;
    //        if (!string.IsNullOrEmpty(lastSku) && lastSku.Contains('-'))
    //        {
    //            var parts = lastSku.Split('-');
    //            if (int.TryParse(parts.Last(), out var num))
    //                nextNumber = num + 1;
    //        }

    //        product.SKU = $"{prefix}-{nextNumber:D3}";

    //        var result = await _unitOfWork.Repository<ProductEntity>().AddAsync(product);
    //        await _unitOfWork.SaveChangesAsync();

    //        return OperationResult.SuccessResult(
    //            ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Product)),
    //            result);
    //    }
    //    catch (Exception ex)
    //    {
    //        if (!string.IsNullOrEmpty(newPicture))
    //            await _blob.DeleteFileByUrlAsync(newPicture);

    //        return OperationResult.FailureResult(
    //            ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Product)) + ": " + ex.Message);
    //    }
    //}
    #endregion 

    public async Task<OperationResult> CreateAsync(CreateProductModel model)
    {
        string? newPicture = null;

        try
        {
            // 1️⃣ Kiểm tra SKU bắt buộc nhập
            if (string.IsNullOrWhiteSpace(model.SKU))
                throw new Exception("Vui lòng nhập mã SKU.");

            var inputSku = model.SKU.Trim().ToLower();

            // 2️⃣ Kiểm tra SKU trùng trong Product
            var existedProduct = await _unitOfWork.Repository<ProductEntity>()
                .GetQueryable()
                .AnyAsync(x => x.SKU.ToLower() == inputSku);

            if (existedProduct)
                throw new Exception($"SKU '{model.SKU}' đã tồn tại, vui lòng nhập mã khác.");

            // 3️⃣ Kiểm tra SKU trùng với Ingredient.Code
            var existedIngredient = await _unitOfWork.Repository<IngredientEntity>()
                .GetQueryable()
                .AnyAsync(x => x.Code.ToLower() == inputSku);

            if (existedIngredient)
                throw new Exception($"Mã '{model.SKU}' bị trùng với mã nhiên liệu, vui lòng nhập mã khác.");

            // 4️⃣ Lấy tên nhóm sản phẩm
            var productGroupName = await _unitOfWork
                .Repository<ProductGroupEntity>()
                .GetQueryable()
                .Where(x => x.Id == model.ProductGroupId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            if (productGroupName is null)
                throw new NotFoundException(
                    ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(ProductGroup), model.ProductGroupId));

            // 5️⃣ Upload hình
            if (model.Picture != null && model.Picture.Length > 0)
            {
                // Có ảnh → upload lên Blob
                newPicture = await _blob.UploadFileAsync(AppConstants.BlobContainer, model.Picture);
            }
            else
            {
                // Không có ảnh → dùng ảnh mặc định (bạn phải upload sẵn 1 ảnh này lên Blob trước)
                newPicture = "https://yourdomain.blob.core.windows.net/images/default-product.png";
                // Hoặc nếu muốn để trống hoàn toàn:
                // uploadedPictureUrl = null;
            }
            //newPicture = await _blob.UploadFileAsync(AppConstants.BlobContainer, model.Picture!);

            // 6️⃣ Mapping sang entity
            var product = _mapper.Map<ProductEntity>(model);
            product.Picture = newPicture;
            product.SKU = model.SKU.Trim(); // gán SKU thủ công
            product.Unit = Enum.TryParse<UnitType>(model.Unit, out var unitType) ? unitType : UnitType.Other;

            // 7️⃣ Lưu DB
            var result = await _unitOfWork.Repository<ProductEntity>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            //Thêm logic tự động thêm vào inven với số lượng 0 khi tạo mới sp
            // Lấy danh sách cửa hàng CHƯA có tồn kho của sản phẩm này
            var storeIdsWithoutInventory = await _unitOfWork.Repository<StoreEntity>()
                .GetQueryable()
                .Where(s => !_unitOfWork.Repository<InventoryEntity>()
                    .GetQueryable()
                    .Any(i => i.ProductId == product.Id && i.StoreId == s.Id))
                .Select(s => s.Id)
                .ToListAsync();
            // bắt đầu check từng store
            if (storeIdsWithoutInventory.Any())
            {
                var inventories = storeIdsWithoutInventory.Select(storeId => new InventoryEntity
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    StoreId = storeId,
                    Quantity = 0
                }).ToList();

                await _unitOfWork.Repository<InventoryEntity>().AddRangeAsync(inventories);
                await _unitOfWork.SaveChangesAsync();
            }

            return OperationResult.SuccessResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Product)),
                result);
        }
        catch (Exception ex)
        {
            // 8️⃣ Xóa hình nếu lỗi
            if (!string.IsNullOrEmpty(newPicture))
                await _blob.DeleteFileByUrlAsync(newPicture);

            return OperationResult.FailureResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Product)) + ": " + ex.Message);
        }
    }

    public async Task<OperationResult> CreateAsync(List<CreateMultipleProductModel> model)
    {
        var uploadFiles = new ConcurrentBag<string>();
        var productToSave = new ConcurrentBag<ProductEntity>();
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10 };

        try
        {
            await Parallel.ForEachAsync(model, parallelOptions, async (item, cancellationToken) =>
            {
                var newPicture = await _blob.UploadFileAsync(AppConstants.BlobContainer, item.Pictures.FirstOrDefault()!);
                uploadFiles.Add(newPicture);
                var product = _mapper.Map<ProductEntity>(item);
                product.Picture = newPicture;

                foreach (var productInfo in item.Products)
                {
                    if (Enum.TryParse<UnitType>(productInfo.Unit, out var unitType))
                    {
                        product.Unit = unitType;
                    }
                    else
                    {
                        product.Unit = UnitType.Other;
                    }
                }

                productToSave.Add(product);
            });
            //var result = await _unitOfWork.Repository<ProductEntity>().AddRangeAsync(productToSave.ToList());
            //return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Product)), result);

            // 1. Thêm tất cả sản phẩm trước
            await _unitOfWork.Repository<ProductEntity>().AddRangeAsync(productToSave.ToList());
            await _unitOfWork.SaveChangesAsync(); // cần để có Id của các product mới

            // 2. Lấy danh sách tất cả cửa hàng (chỉ query 1 lần)
            var storeIds = await _unitOfWork.Repository<StoreEntity>()
                .GetQueryable()
                .Select(s => s.Id)
                .ToListAsync();

            // Lấy tất cả ProductId vừa tạo
            var productIds = productToSave.Select(p => p.Id).ToList();

            // Query 1 lần: lấy tất cả cặp (ProductId, StoreId) đã tồn tại trong DB
            var existingPairs = await _unitOfWork.Repository<InventoryEntity>()
                .GetQueryable()
                .Where(i => productIds.Contains(i.ProductId))
                .Select(i => new { i.ProductId, i.StoreId })
                .ToListAsync();

            var existingLookup = existingPairs
                .ToLookup(x => x.ProductId, x => x.StoreId);

            // Tạo tồn kho chỉ cho những cặp chưa tồn tại
            var inventoriesToAdd = new List<InventoryEntity>();

            foreach (var product in productToSave)
            {
                var existingStoreIds = existingLookup[product.Id].ToHashSet();

                foreach (var storeId in storeIds)
                {
                    if (!existingStoreIds.Contains(storeId))
                    {
                        inventoriesToAdd.Add(new InventoryEntity
                        {
                            Id = Guid.NewGuid(),
                            ProductId = product.Id,
                            StoreId = storeId,
                            Quantity = 0
                        });
                    }
                }
            }

            if (inventoriesToAdd.Any())
            {
                await _unitOfWork.Repository<InventoryEntity>().AddRangeAsync(inventoriesToAdd);
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.SaveChangesAsync(); // lưu tồn kho

            return OperationResult.SuccessResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Product)),
                productToSave.Count);

        }
        catch (Exception ex)
        {
            foreach (var file in uploadFiles)
            {
                await _blob.DeleteFileAsync(AppConstants.BlobContainer, file);
            }
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(Product)) + ": " + ex.Message);
        }
    }
    public Task<OperationResult> CreateAsync(List<CreateProductInfoModel> model)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult> UpdateAsync(Guid id, ProductUpdateModel model,Guid userId)
    {
        var product = await _unitOfWork.Repository<ProductEntity>().GetPurchaseOrderByIdAsync(id);
        if (product is null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Product), id));
        string? newPicture = null!;
        var oldPrice = product.Price; //Lưu giá cũ

        try
        {
            _mapper.Map(model, product);

            if (model.Picture != null)
            {
                newPicture = await _blob.UploadFileAsync(AppConstants.BlobContainer, model.Picture);
                await _blob.DeleteFileAsync(AppConstants.BlobContainer, product.Picture!);

                product.Picture = newPicture;
            }
            if (Enum.TryParse<UnitType>(model.Unit, out var unitType))
            {
                product.Unit = unitType;
            }
            else
            {
                product.Unit = UnitType.Other;
            }

            if (model.Price != oldPrice)
            {
                var history = ProductPriceHistoryEntity.Create(
                    productId: id,
                    oldPrice: oldPrice,
                    newPrice: model.Price,
                    changedByUserId: userId
                );
                await _unitOfWork.Repository<ProductPriceHistoryEntity>().AddAsync(history);
            }

            var result = await _unitOfWork.Repository<ProductEntity>().UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(Product)));
        }
        catch (Exception ex)
        {
            await _blob.DeleteFileAsync(AppConstants.BlobContainer, newPicture);
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(Product)) + ": " + ex.Message);
        }
    }

    public async Task<OperationResult> DeleteOrRestoreProductAsync(Guid id)
    {
        var product = await _unitOfWork.Repository<ProductEntity>().GetPurchaseOrderByIdAsync(id);
        if (product is null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Product), id));

        if (product.IsActive == true)
        {
            product.IsActive = false;
        }
        else
        {
            product.IsActive = true;
        }
        var result = await _unitOfWork.Repository<ProductEntity>().UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
        if (!result)
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(Product)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.DeletedSuccess, nameof(User)));
    }
    private string GetUnitDisplayName(UnitType unit)
    {
        return unit switch
        {
            UnitType.Gram => "Gam",
            UnitType.Piece => "Cái",
            UnitType.Milliliter => "Ml",
            UnitType.Slice => "Lát",
            UnitType.Bag => "Túi",
            UnitType.Fruit => "Quả",
            UnitType.Loaf => "Ổ",
            UnitType.Stick => "cây",
            UnitType.Portion => "phần",
            UnitType.Chunk => "Miếng",
            _ => "Khác"
        };
    }

    public async Task<(bool IsValid, string? SuggestSku, string? Reason)> CheckSkuAsync(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new Exception("SKU không được để trống.");

        var input = sku.Trim().ToUpper();

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

        // 1️⃣ Kiểm tra SKU có tồn tại trong Product
        var existedProductSku = await _unitOfWork.Repository<ProductEntity>()
            .GetQueryable()
            .AnyAsync(x => x.SKU.Trim().ToUpper() == input);

        // 2️⃣ Kiểm tra SKU có trùng Code nào của Ingredient
        var existedIngredientCode = await _unitOfWork.Repository<IngredientEntity>()
            .GetQueryable()
            .AnyAsync(x => x.Code.Trim().ToUpper() == input);

        if (!existedProductSku && !existedIngredientCode)
        {
            // Không trùng gì → hợp lệ
            return (true, null, null);
        }

        // Xác định Reason
        string reason = existedProductSku ? "Trùng với SKU sản phẩm" : "Trùng với mã nhiên liệu";

        // 3️⃣ Lấy tất cả SKU bắt đầu bằng prefix hoặc chính prefix
        var upperPrefix = (prefix + "-").ToUpper();
        var allSkus = await _unitOfWork.Repository<ProductEntity>()
            .GetQueryable()
            .Where(x => x.SKU.Trim().ToUpper() == prefix.ToUpper() || x.SKU.Trim().ToUpper().StartsWith(upperPrefix))
            .Select(x => x.SKU)
            .ToListAsync();

        int maxNumber = 0;
        foreach (var code in allSkus)
        {
            if (code.Contains('-'))
            {
                var parts = code.Split('-');
                if (int.TryParse(parts.Last(), out var num))
                    maxNumber = Math.Max(maxNumber, num);
            }
            else
            {
                maxNumber = Math.Max(maxNumber, 0); // prefix không có số
            }
        }

        int nextNumber = Math.Max(inputNumber + 1, maxNumber + 1);
        string suggest = $"{prefix}-{nextNumber:D3}";

        return (false, suggest, reason);
    }
}
