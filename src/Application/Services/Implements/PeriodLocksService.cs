using Domain.Models.PeriodLock;

namespace Application;

[RegisterService(typeof(IPeriodLocksService))]
public class PeriodLocksService(
    //IPeriodLocksRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper
    ) : IPeriodLocksService
{

    public async Task<string> GetPeriodLocksAsync(Guid storeId)
    {
        return await _unitOfWork.Repository<PeriodLockEntity>()
        .GetQueryable()
        .Where(x => x.StoreId == storeId)
        .OrderByDescending(x => x.CreateDate)
        .Select(x => x.Status.ToString())
        .FirstOrDefaultAsync() ?? throw new NotFoundException($"Không tìm thấy trạng thái lock của cửa hàng {storeId}");
    }
    public async Task<OperationResult> OpenPeriodLockAsync(CreatePeriodLockModel model)
    {
        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                var existingOpen = await _unitOfWork.Repository<PeriodLockEntity>()
                    .GetByConditionAsync(x => x.StoreId == model.StoreId && x.Status == ShiftPeriodStatusType.Started);

                if (existingOpen != null)
                {
                    throw new BadRequestException("Đã có ca đang mở. Vui lòng khóa trước khi mở ca mới.");
                }

                var periodLock = _mapper.Map<PeriodLockEntity>(model);
                periodLock.Status = ShiftPeriodStatusType.Started;
                await _unitOfWork.Repository<PeriodLockEntity>().AddAsync(periodLock);

                #region Comment tách Logic lưu số lượng kho khi mở sổ
                //var oldLocks = await _repository.GetInventoryPeriodLockOfStoreAsync(model.StoreId);

                //var inventoryProducts = await _unitOfWork.Repository<InventoryEntity>()
                //    .GetListByConditionAsync(x => x.StoreId == model.StoreId && x.ProductId != Guid.Empty);
                //var inventoryIngredients = await _unitOfWork.Repository<IngredientInventoryEntity>()
                //    .GetListByConditionAsync(x => x.StoreId == model.StoreId && x.IngredientId != Guid.Empty);

                //var lockEntries = new List<InventoryPeriodLockEntity>();
                //foreach (var p in inventoryProducts)
                //{
                //    lockEntries.Add(new InventoryPeriodLockEntity
                //    {
                //        Id = Guid.NewGuid(),
                //        StoreId = model.StoreId,
                //        ProductId = p.ProductId,
                //        OpeningQuantity = p.Quantity,
                //        ClosingQuantity = 0,
                //        EditStatus = InventoryEditStatusType.NotEdited
                //    });
                //}
                //foreach (var i in inventoryIngredients)
                //{
                //    lockEntries.Add(new InventoryPeriodLockEntity
                //    {
                //        Id = Guid.NewGuid(),
                //        StoreId = model.StoreId,
                //        IngredientId = i.IngredientId,
                //        OpeningQuantity = i.Quantity,
                //        ClosingQuantity = 0,
                //        EditStatus = InventoryEditStatusType.NotEdited
                //    });
                //}

                //await _unitOfWork.Repository<InventoryPeriodLockEntity>().AddRangeAsync(lockEntries);
                #endregion

                await _unitOfWork.SaveChangesAsync();
            });

            return OperationResult.SuccessResult("Mở sổ kho thành công.");
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult("Lỗi mở sổ kho: " + ex.Message);
        }
    }

    public async Task<OperationResult> ClosePeriodLockAsync(ClosePeriodLockModel model)
    {
        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                var periodLock = await _unitOfWork.Repository<PeriodLockEntity>()
                    .GetByConditionAsync(x => x.StoreId == model.StoreId && x.Status == ShiftPeriodStatusType.Started);

                if (periodLock == null)
                {
                    throw new NotFoundException($"Không tìm thấy ca đang mở cho cửa hàng ID: {model.StoreId}");
                }

                _mapper.Map(model, periodLock);
                periodLock.Status = ShiftPeriodStatusType.Locked;

                #region Comment Logic lưu số lượng kho khi khoá sổ
                //var inventoryLocks = await _repository.GetInventoryPeriodLockOfStoreAsync(model.StoreId);

                //var productIds = inventoryLocks.Where(x => x.ProductId.HasValue).Select(x => x.ProductId!.Value).Distinct().ToList();
                //var ingredientIds = inventoryLocks.Where(x => x.IngredientId.HasValue).Select(x => x.IngredientId!.Value).Distinct().ToList();

                //var inventories = await _unitOfWork.Repository<InventoryEntity>()
                //    .GetListByConditionAsync(x => x.StoreId == model.StoreId && productIds.Contains(x.ProductId));

                //var ingredientInventories = await _unitOfWork.Repository<IngredientInventoryEntity>()
                //    .GetListByConditionAsync(x => x.StoreId == model.StoreId && ingredientIds.Contains(x.IngredientId));

                //var inventoryDict = inventories.ToDictionary(x => x.ProductId, x => x);
                //var ingredientDict = ingredientInventories.ToDictionary(x => x.IngredientId, x => x);

                //foreach (var item in inventoryLocks)
                //{
                //    if (item.ProductId.HasValue && inventoryDict.TryGetValue(item.ProductId.Value, out var inv))
                //    {
                //        item.ClosingQuantity = inv.Quantity;
                //        item.OpeningQuantity = inv.Quantity;
                //    }

                //    if (item.IngredientId.HasValue && ingredientDict.TryGetValue(item.IngredientId.Value, out var ingInv))
                //    {
                //        item.ClosingQuantity = ingInv.Quantity;
                //        item.OpeningQuantity = ingInv.Quantity;
                //    }

                //    item.EditStatus = InventoryEditStatusType.NotEdited;
                //    _unitOfWork.Repository<InventoryPeriodLockEntity>().UpdateWithoutSave(item);
                //}

                //if (model.Note != null && model.Note.Count > 0)
                //{
                //    await _unitOfWork.Repository<PeriodLockNoteEntity>().AddRangeAsync(periodLock.Note);
                //}
                #endregion

                await _unitOfWork.SaveChangesAsync();
            });

            return OperationResult.SuccessResult("Khóa sổ kho thành công.");
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult("Lỗi khóa sổ kho: " + ex.Message);
        }
    }

    public async Task<OperationResult> UpdateInventoryQuantityLockAsync(UpdateInventoryQuantityLockModel model)
    {
        var inventoryPeriodLockEntity = await _unitOfWork.Repository<InventoryPeriodLockEntity>()
            .GetByConditionAsync(x => x.StoreId == model.StoreId && x.ProductId == model.ProductId);
        if (inventoryPeriodLockEntity == null)
            return OperationResult.FailureResult("Inventory Period Lock not found");

        var inventoryEntity = await _unitOfWork.Repository<InventoryEntity>()
            .GetByConditionAsync(x => x.StoreId == model.StoreId && x.ProductId == model.ProductId);

        if (inventoryPeriodLockEntity == null)
            return OperationResult.FailureResult("Inventory lock not found");

        _mapper.Map(model, inventoryPeriodLockEntity);
        inventoryPeriodLockEntity.EditStatus = InventoryEditStatusType.Edited;
        await _unitOfWork.Repository<InventoryPeriodLockEntity>().UpdateAsync(inventoryPeriodLockEntity);

        inventoryEntity!.Quantity = model.ClosingQuantity;
        await _unitOfWork.Repository<InventoryEntity>().UpdateAsync(inventoryEntity!);
        if (await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.SuccessResult(
                ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(InventoryPeriodLockEntity)));
        }
        return OperationResult.FailureResult(
            ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(InventoryPeriodLockEntity)));
    }

    public async Task<OperationResult> UpdateIngredientInventoryQuantityLockAsync(UpdateIngredientInventoryQuantityLockModel model)
    {
        var inventoryPeriodLockEntity = await _unitOfWork.Repository<InventoryPeriodLockEntity>()
            .GetByConditionAsync(x => x.StoreId == model.StoreId && x.IngredientId == model.IngredientId);
        if (inventoryPeriodLockEntity == null)
            return OperationResult.FailureResult("Inventory Period Lock lock not found");

        var inventoryEntity = await _unitOfWork.Repository<IngredientInventoryEntity>()
            .GetByConditionAsync(x => x.StoreId == model.StoreId && x.IngredientId == model.IngredientId);

        if (inventoryEntity == null)
            return OperationResult.FailureResult("Ingredient Inventory lock not found");


        _mapper.Map(model, inventoryPeriodLockEntity);
        inventoryPeriodLockEntity.EditStatus = InventoryEditStatusType.Edited;
        await _unitOfWork.Repository<InventoryPeriodLockEntity>().UpdateAsync(inventoryPeriodLockEntity);


        inventoryEntity.Quantity = model.ClosingQuantity;
        await _unitOfWork.Repository<IngredientInventoryEntity>().UpdateAsync(inventoryEntity);
        if (await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.SuccessResult(
                ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(InventoryPeriodLockEntity)));
        }
        return OperationResult.FailureResult(
            ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(InventoryPeriodLockEntity)));
    }
    public async Task<OperationResult> GetPeriodLocksByFilterAsync(FilterPeriodLockModel model)
    {
        try
        {
            var fromDate = model.FromDate.Date;
            var toDate = model.ToDate.Date.AddDays(1);

            var query = _unitOfWork.Repository<PeriodLockEntity>()
                .GetQueryable()
                .Include(x => x.Note)
                .Where(x => x.CreateDate >= fromDate && x.CreateDate < toDate);

            // Lọc theo UserId (nullable)
            if (model.UserId.HasValue && model.UserId.Value != Guid.Empty)
            {
                query = query.Where(x => x.UserId == model.UserId.Value);
            }
            else if (model.StoreId.HasValue && model.StoreId.Value != Guid.Empty)
            {
                query = query.Where(x => x.StoreId == model.StoreId.Value);
            }

            var data = await query
                .OrderByDescending(x => x.CreateDate)
                .ToListAsync();

            // UserId trong entity KHÔNG nullable => không cần HasValue
            var userIds = data
                .Select(x => x.UserId)
                .Distinct()
                .ToList();

            var users = await _unitOfWork.Repository<UserEntity>()
                .GetQueryable()
                .Where(x => userIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.Email);

            var result = _mapper.Map<List<PeriodLockWithNotesDto>>(data);

            // Gán email vào DTO
            foreach (var item in result)
            {
                if (users.TryGetValue(item.UserId, out var email))
                {
                    item.Email = email;
                }
            }

            return OperationResult.SuccessResult(null, result);
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult("Lỗi lấy dữ liệu khóa sổ: " + ex.Message);
        }
    }






}
