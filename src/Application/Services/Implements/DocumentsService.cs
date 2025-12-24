namespace Application;

[RegisterService(typeof(IDocumentsService))]
public class DocumentsService(
    IDocumentsRepository _repository,
    IProductsRepository _productsRepository,
    IIngredientsRepository _ingredientsRepository,
    IUnitOfWork _unitOfWork,
    NotificationHelper _notificationHelper,
    IMapper _mapper) : IDocumentsService
{
    public async Task<QueryResult<Document>> GetDocumentsWithPagingAsync(Guid storeId, QueryInfo queryInfo, QueryDocument queryDate)
    {
        return await _repository.GetDocumentsWithPagingAsync(storeId, queryInfo, queryDate);
    }
    public async Task<QueryResult<DocumentSupplierForExportrotationModel>> GetDocumentsForExportrotationWithPagingAsync(Guid storeId, QueryInfo queryInfo, DocumentForExportrotationQuery query)
    => await _repository.GetDocumentsForExportrotationWithPagingAsync(storeId, queryInfo, query);
    public async Task<DocumentSupplierModel> GetDocumentByIdAsync(Guid storeId, Guid id)
    {
        return await _repository.GetDocumentByIdAsync(storeId, id)
               ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(DocumentSupplierModel), id));
    }
    public async Task<DocumentSupplierForExportrotationModel> GetDocumentForExportrotationByIdAsync(Guid storeId, Guid id)
        => await _repository.GetDocumentForExportrotationByIdAsync(storeId, id);

    public async Task<OperationResult> CheckCodeExistsAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return OperationResult.FailureResult("Code Null");

        var normalized = code.Trim().ToLower();

        var productCodes = await _productsRepository.GetProductSkusAsync();
        var ingredientCodes = await _ingredientsRepository.GetIngredientCodesAsync();

        if (productCodes.Contains(normalized))
            return OperationResult.SuccessResult("product");

        if (ingredientCodes.Contains(normalized))
            return OperationResult.SuccessResult("ingredient");

        return OperationResult.FailureResult("Code does not exist");
    }

    public async Task<OperationResult> GetItemByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return OperationResult.FailureResult("Code is null or empty");

        var normalized = code.Trim().ToLower();

        // Load danh sách mã để phân loại
        var productCodes = new HashSet<string>(
            (await _productsRepository.GetProductSkusAsync())
            .Select(x => x.ToLower())
        );

        var ingredientCodes = new HashSet<string>(
            (await _ingredientsRepository.GetIngredientCodesAsync())
            .Select(x => x.ToLower())
        );

        bool isProduct = productCodes.Contains(normalized);
        bool isIngredient = ingredientCodes.Contains(normalized);

        if (!isProduct && !isIngredient)
            return OperationResult.FailureResult($"Code '{code}' does not exist");


        // -------------------------------
        // 🔍 PRODUCT → dùng GetBySkuAsync
        // -------------------------------
        if (isProduct)
        {
            var product = await _productsRepository.GetBySkuAsync(normalized);

            if (product == null)
                return OperationResult.FailureResult($"Product '{code}' not found");

            return OperationResult.SuccessResult("product", product);
        }


        // -------------------------------
        // 🔍 INGREDIENT → dùng GetIngredientByCodeAsync
        // -------------------------------
        if (isIngredient)
        {
            var ingredient = await _ingredientsRepository.GetIngredientByCodeAsync(normalized);

            if (ingredient == null)
                return OperationResult.FailureResult($"Ingredient '{code}' not found");

            return OperationResult.SuccessResult("ingredient", ingredient);
        }

        // Không thể vào đây nhưng để an toàn
        return OperationResult.FailureResult("Unknown error occurred");
    }

    #region  createDocument cũ
    //public async Task<OperationResult> CreateDocumentAsync(CreateDocumentSupplierModel model)
    //{
    //    try
    //    {
    //        List<Guid> productIds = new List<Guid>();
    //        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
    //        {
    //            var storeEntity = await _unitOfWork.Repository<StoreEntity>().GetByIdAsync(model.StoreId);
    //            Enum.TryParse<TransType>(model.TransType, out var transType);
    //            DocumentStatusType documentStatusType = DocumentStatusType.Completed;
    //            var documentResult = await InsertDocumentAsync(model.Document, documentStatusType, transType, storeEntity.Name);
    //            var mappedItems = _mapper.Map<List<DocumentSupplierEntity>>(model.Products);

    //            foreach (var item in mappedItems)
    //            {
    //                item.DocumentId = documentResult.Id;
    //            }

    //            var productCodes = await _productsRepository.GetProductSkusAsync();

    //            var ingredientCodes = await _ingredientsRepository.GetIngredientCodesAsync();

    //            var unknownRows = new List<AddProductOrIngredientInventoryModel>();

    //            for (int i = 0; i < model.Products.Count; i++)
    //            {
    //                var row = model.Products[i];
    //                var item = mappedItems[i];
    //                item.DocumentId = documentResult.Id;

    //                var code = row.Code!.Trim().ToLower();
    //                if (productCodes.Contains(code) && !ingredientCodes.Contains(code))
    //                {
    //                    var product = await _unitOfWork.Repository<ProductEntity>()
    //                        .GetByConditionAsync(x => x.SKU.ToLower() == code);
    //                    productIds.Add(product!.Id);
    //                    item.StoreId = model.StoreId;
    //                    item.TransType = transType;
    //                    item.ProductId = product!.Id;
    //                    await ImportProductInventoryAsync(product.Id, model.StoreId, row.Quantity, transType);
    //                }
    //                else if (ingredientCodes.Contains(code) && !productCodes.Contains(code))
    //                {
    //                    var ingredient = await _unitOfWork.Repository<IngredientEntity>()
    //                        .GetByConditionAsync(x => x.Code.ToLower() == code);
    //                    item.StoreId = model.StoreId;
    //                    item.TransType = transType;
    //                    item.IngredientId = ingredient!.Id;
    //                    await ImportIngredientInventoryAsync(ingredient.Id, model.StoreId, row.Quantity, transType);
    //                }
    //            }
    //            var result = await _unitOfWork.Repository<DocumentSupplierEntity>().AddRangeAsync(mappedItems);
    //        });

    //        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Document)),
    //                new DocumentCreatedData { ProductIds = productIds });
    //    }
    //    catch (Exception ex)
    //    {
    //        return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, ex.InnerException?.Message ?? ex.Message));
    //    }
    //}
    #endregion
    public async Task<OperationResult> CreateDocumentAsync(CreateDocumentSupplierModel model, Guid userId)
    {
        try
        {
            List<Guid> productIds = new();

            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                var storeEntity = await _unitOfWork.Repository<StoreEntity>().GetByIdAsync(model.StoreId);
                Enum.TryParse<TransType>(model.TransType, out var transType);
                DocumentStatusType documentStatusType = DocumentStatusType.Completed;

                var documentResult = await InsertDocumentAsync(model.Document, documentStatusType, transType, storeEntity.Name, userId);

                var mappedItems = _mapper.Map<List<DocumentSupplierEntity>>(model.Products);
                foreach (var item in mappedItems)
                    item.DocumentId = documentResult.Id;

                // -----------------------------------------
                // 🧠 Load code list 1 lần và convert HashSet
                // -----------------------------------------
                var productCodes = new HashSet<string>(
                    (await _productsRepository.GetProductSkusAsync())
                    .Select(x => x.ToLower())
                );

                var ingredientCodes = new HashSet<string>(
                    (await _ingredientsRepository.GetIngredientCodesAsync())
                    .Select(x => x.ToLower())
                );

                for (int i = 0; i < model.Products.Count; i++)
                {
                    var row = model.Products[i];
                    var item = mappedItems[i];

                    var code = row.Code!.Trim().ToLower();

                    // 1️⃣ Check mã tồn tại
                    if (!productCodes.Contains(code) && !ingredientCodes.Contains(code))
                        throw new Exception($"Code '{row.Code}' does not exist");

                    item.DocumentId = documentResult.Id;
                    item.StoreId = model.StoreId;
                    item.TransType = transType;

                    // 2️⃣ Phân loại chính xác bằng logic (không dùng message)
                    if (productCodes.Contains(code))
                    {
                        var product = await _unitOfWork.Repository<ProductEntity>()
                            .GetByConditionAsync(x => x.SKU.ToLower() == code);

                        if (product == null)
                            throw new Exception($"Product with code '{row.Code}' not found.");

                        productIds.Add(product.Id);
                        item.ProductId = product.Id;

                        await ImportProductInventoryAsync(product.Id, model.StoreId, row.Quantity, transType);
                    }
                    else if (ingredientCodes.Contains(code))
                    {
                        var ingredient = await _unitOfWork.Repository<IngredientEntity>()
                            .GetByConditionAsync(x => x.Code.ToLower() == code);

                        if (ingredient == null)
                            throw new Exception($"Ingredient with code '{row.Code}' not found.");

                        item.IngredientId = ingredient.Id;

                        await ImportIngredientInventoryAsync(ingredient.Id, model.StoreId, row.Quantity, transType);
                    }
                }

                await _unitOfWork.Repository<DocumentSupplierEntity>().AddRangeAsync(mappedItems);
            });

            return OperationResult.SuccessResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(Document)),
                new DocumentCreatedData { ProductIds = productIds }
            );
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult(
                ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, ex.InnerException?.Message ?? ex.Message)
            );
        }
    }
    public async Task<OperationResult> CreateDocumentForExportrotationAsync(CreateDocumentSupplierForExportrotationModel model, Guid userId)
    {
        if (model.FromStoreId == model.ToStoreId)
            return OperationResult.FailureResult("Cửa hàng không thể mượn hàng từ chính mình.");
        
        if (!await _unitOfWork.Repository<StoreEntity>().CheckExistByIdAsync(model.ToStoreId))
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(StoreEntity), model.ToStoreId));

        var storeEntity = await _unitOfWork.Repository<StoreEntity>().GetByIdAsync(model.FromStoreId);
        TransType transType = TransType.Exportrotation;
        DocumentStatusType documentStatusType = DocumentStatusType.Pending;
        var documentResult = await InsertDocumentAsync(model.Document, documentStatusType, transType, storeEntity.Name, userId);
        var mappedItems = _mapper.Map<List<DocumentSupplierEntity>>(model.Products);

        foreach (var (row, item) in model.Products.Zip(mappedItems))
        {
            item.Id = Guid.NewGuid();
            item.DocumentId = documentResult.Id;
            item.StoreId = model.FromStoreId;
            item.ToStoreId = model.ToStoreId;
            item.TransType = transType;
            item.Quantity = row.Quantity;

            if (row.IsProduct)
                item.ProductId = row.Id;
            else
                item.IngredientId = row.Id;
        }
        var toStoreName = (await _unitOfWork.Repository<StoreEntity>().GetByIdAsync(model.ToStoreId))?.Name ?? "Cửa hàng đã xóa";

        var result = await _unitOfWork.Repository<DocumentSupplierEntity>().AddRangeAsync(mappedItems);
        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(DocumentEntity)));

        await _notificationHelper.SendAndSaveAsync(
            model.ToStoreId,
            "Đề nghị mượn hàng mới",
            $"Cửa hàng {storeEntity.Name} đã gửi đề nghị mượn hàng đến bạn.",
            documentResult.Id,
            NotificationType.Document
        );

        await _notificationHelper.SendAndSaveAsync(
            model.FromStoreId,
            "Đã gửi thành công đề nghị mượn hàng",
            $"Bạn đã gửi đề nghị mượn hàng đến cửa hàng \"{toStoreName}\".",
            documentResult.Id,
            NotificationType.Document
        );
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(DocumentEntity)));
    }
    public async Task<OperationResult> UpdateDocumentStatusAsync(Guid documentId, UpdateDocumentStatusModel model)
    {
        var documentEntity = await _unitOfWork.Repository<DocumentEntity>().GetByIdAsync(documentId);

        var documentSupplierEntities = await _unitOfWork.Repository<DocumentSupplierEntity>()
            .GetListByConditionAsync(x => x.DocumentId == documentId && x.ToStoreId == model.ToStoreId);

        if (documentSupplierEntities.Count() == 0)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(DocumentSupplierEntity), documentId));

        var productIds = documentSupplierEntities.Select(x => x.ProductId).Where(id => id.HasValue).Select(id => id!.Value).ToHashSet();
        var ingredientIds = documentSupplierEntities.Select(x => x.IngredientId).Where(id => id.HasValue).Select(id => id!.Value).ToHashSet();

        var fromStoreName = (await _unitOfWork.Repository<StoreEntity>().GetByIdAsync(model.FromStoreId))?.Name ?? "Cửa hàng đã xóa";
        var toStoreName = (await _unitOfWork.Repository<StoreEntity>().GetByIdAsync(model.ToStoreId))?.Name ?? "Cửa hàng đã xóa";

        if (model.Status == DocumentStatusType.Canceled.ToString())
        {
            documentEntity.Status = DocumentStatusType.Canceled;
            await _unitOfWork.Repository<DocumentEntity>().UpdateAsync(documentEntity);
            await _notificationHelper.SendAndSaveAsync(
                model.ToStoreId,
                "Đề nghị mượn hàng bị hủy",
                //$"Đề nghị mượn hàng ID {documentId} đã bị hủy.",
                $"Đề nghị mượn hàng từ cửa hàng \"{fromStoreName}\" đã bị hủy.",
                documentId,
                NotificationType.Document
            );
            return OperationResult.SuccessResult("Hủy đề nghị thành công");
        }

        documentEntity.Status = DocumentStatusType.Completed;
        await _unitOfWork.Repository<DocumentEntity>().UpdateAsync(documentEntity);

        foreach (var item in documentSupplierEntities)
        {
            // Lấy kho của hai cửa hàng
            var fromStoreId = item.StoreId;
            var toStoreId = item.ToStoreId;

            if (item.ProductId.HasValue)
            {
                var productId = item.ProductId.Value;
                await UpdateInventoryAsync(productId, item.Quantity, fromStoreId, toStoreId ?? Guid.Empty);
            }
            else if (item.IngredientId.HasValue)
            {
                var ingredientId = item.IngredientId.Value;
                await UpdateIngredientInventoryAsync(ingredientId, item.Quantity, fromStoreId, toStoreId ?? Guid.Empty);
            }
        }

        if (!await _unitOfWork.SaveChangesAsync())
            return OperationResult.FailureResult("Cập nhật kho thất bại");

        await _notificationHelper.SendAndSaveAsync(
            model.FromStoreId,
            "Đề nghị mượn hàng hoàn thành",
            //$"Cửa hàng ID {model.ToStoreId} đã nhận được hàng.",
            $"Cửa hàng \"{toStoreName}\" đã nhận được hàng.",
            documentId,
            NotificationType.Document
        );

        await _notificationHelper.SendAndSaveAsync(
            model.ToStoreId,
            "Nhận hàng thành công",
            //$"Bạn đã nhận thành công đề nghị mượn hàng từ cửa hàng ID {model.FromStoreId}.",
            $"Bạn đã nhận thành công đề nghị mượn hàng từ cửa hàng \"{fromStoreName}\".",
            documentId,
            NotificationType.Document
        );
        return OperationResult.SuccessResult("Cập nhật kho thành công");
    }

    private async Task<DocumentEntity> InsertDocumentAsync(CreateDocumentModel model, DocumentStatusType documentStatusType, TransType transType, string storeName, Guid userId)
    {
        var documentEntity = _mapper.Map<DocumentEntity>(model);
        string type = transType.GetDescription();

        documentEntity.DocumentName = TextHelper.GenerateDocumentName(type, storeName);
        documentEntity.Status = documentStatusType;
        documentEntity.UserId = userId;
        var documentResult = await _unitOfWork.Repository<DocumentEntity>().AddAsync(documentEntity);
        return documentResult;
    }
    private async Task ImportProductInventoryAsync(Guid productId, Guid storeId, decimal quantity, TransType transType)
    {
        var inventory = await _unitOfWork.Repository<InventoryEntity>()
                .GetByConditionAsync(x => x.ProductId == productId && x.StoreId == storeId);

        if (inventory == null)
        {
            if (transType != TransType.GoodsReceipt)
            {
                throw new BadRequestException("Product not in inventory");
            }
            var inventoryEntity = new InventoryEntity
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                ProductId = productId,
                Quantity = quantity
            };

            await _unitOfWork.Repository<InventoryEntity>().AddAsync(inventoryEntity);
        }
        else
        {
            //inventory.Quantity = Math.Max(CalculatorQuantity(transType, inventory.Quantity, quantity), 0);
            inventory.Quantity = CalculatorQuantity(transType, inventory.Quantity, quantity);
        }
        // không liên quan đến InventoryPeriodLock khi nhập chứng từ , đã bỏ qua logic này bây h bảng này chỉ lưu lock kho cuối ngày thôi.
        //await AddToInventoryPeriodLockAsync(transType, productId, storeId, quantity, isProduct: true);
    }
    private async Task ImportIngredientInventoryAsync(Guid ingredientId, Guid storeId, decimal quantity, TransType transType)
    {
        var inventory = await _unitOfWork.Repository<IngredientInventoryEntity>()
                .GetByConditionAsync(x => x.IngredientId == ingredientId && x.StoreId == storeId);

        if (inventory == null)
        {
            if (transType != TransType.GoodsReceipt)
            {
                throw new BadRequestException("Ingredient not in inventory");
            }
            var ingredientInventoryEntity = new IngredientInventoryEntity
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                IngredientId = ingredientId,
                Quantity = quantity
            };

            await _unitOfWork.Repository<IngredientInventoryEntity>().AddAsync(ingredientInventoryEntity);
        }
        else
        {
            //inventory.Quantity = Math.Max(CalculatorQuantity(transType, inventory.Quantity, quantity), 0);
            inventory.Quantity = CalculatorQuantity(transType, inventory.Quantity, quantity);
        }
        // không liên quan đến InventoryPeriodLock khi nhập chứng từ , đã bỏ qua logic này bây h bảng này chỉ lưu lock kho cuối ngày thôi.
        //await AddToInventoryPeriodLockAsync(transType, ingredientId, storeId, quantity, isProduct: false);
    }

    private async Task UpdateInventoryAsync(Guid productId, decimal quantity, Guid fromStoreId, Guid toStoreId)
    {
        var inventoryRepo = _unitOfWork.Repository<InventoryEntity>();

        var fromInventory = await inventoryRepo.GetByConditionAsync(x => x.ProductId == productId && x.StoreId == fromStoreId);
        if (fromInventory != null)
            fromInventory.Quantity += quantity;
        else
            throw new Exception($"Không tìm thấy sản phẩm {productId} trong kho cửa hàng {fromStoreId}");

        var toInventory = await inventoryRepo.GetByConditionAsync(x => x.ProductId == productId && x.StoreId == toStoreId);
        if (toInventory != null)
            toInventory.Quantity -= quantity;
        
    }
    private async Task UpdateIngredientInventoryAsync(Guid ingredientId, decimal quantity, Guid fromStoreId, Guid toStoreId)
    {
        var ingredientRepo = _unitOfWork.Repository<IngredientInventoryEntity>();

        var fromInventory = await ingredientRepo.GetByConditionAsync(x => x.IngredientId == ingredientId && x.StoreId == fromStoreId);
        if (fromInventory != null)
            fromInventory.Quantity += quantity;
        else
            throw new Exception($"Không tìm thấy nguyên liệu {ingredientId} trong kho cửa hàng {fromStoreId}");

        var toInventory = await ingredientRepo.GetByConditionAsync(x => x.IngredientId == ingredientId && x.StoreId == toStoreId);
        if (toInventory != null)
            toInventory.Quantity -= quantity;
    }

    private async Task<bool> AddToInventoryPeriodLockAsync(TransType transType, Guid id, Guid storeId, decimal quantity, bool isProduct)
    {
        var repo = _unitOfWork.Repository<InventoryPeriodLockEntity>();

        InventoryPeriodLockEntity? existingLock;

        if (isProduct)
        {
            existingLock = await repo.GetByConditionAsync(x => x.StoreId == storeId && x.ProductId == id);
        }
        else
        {
            existingLock = await repo.GetByConditionAsync(x => x.StoreId == storeId && x.IngredientId == id);
        }

        if (existingLock != null)
        {
            if (transType == TransType.GoodsReceipt)
            {
                existingLock.OpeningQuantity += quantity;
            }
            return await repo.UpdateAsync(existingLock);
        }
        else
        {
            var newLock = new InventoryPeriodLockEntity
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
                ProductId = isProduct ? id : null,
                IngredientId = isProduct ? null : id,
                OpeningQuantity = quantity
            };

            await repo.AddAsync(newLock);
            return true;
        }
    }

    private decimal CalculatorQuantity(TransType transType, decimal currentQuantity, decimal quantity)
    {
        return transType switch
        {
            TransType.GoodsReceipt => currentQuantity + quantity,// Nhập hàng
            TransType.ReturnedGoodsReceipt => currentQuantity - quantity, // Nhập trả lại  // Chuyển thành Xuất hoàn kho tổng 
            TransType.AdjustmentEntry => quantity,// Xuất hàng  // chuyển thành nhập điều chỉnh 
            TransType.ReturnOutput => currentQuantity + quantity,// Xuất trả lại
            TransType.CanceledProduct => currentQuantity - quantity,// Hủy sản phẩm
            _ => 0
        };
    }
}
