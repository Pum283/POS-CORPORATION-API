namespace Application;

[RegisterService(typeof(IPurchaseOrdersService))]
public class PurchaseOrdersService(
    IPurchaseOrdersRepository _repository,
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IPurchaseOrdersService
{
    public Task<QueryResult<PurchaseOrder>> GetPurchaseOrderWithPagingAsync(QueryInfo queryInfo)
    {
        return _repository.GetPurchaseOrderWithPagingAsync(queryInfo);
    }

    public async Task<OperationResult> CreatePurchaseOrderAsync(Guid storeId, CreatePurchaseOrderModel model)
    {
        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                var order = _mapper.Map<PurchaseOrderEntity>(model);
                await _unitOfWork.Repository<PurchaseOrderEntity>().AddAsync(order);

                if (model.Status == OrderStatusType.Confirmed)
                {
                    foreach (var item in order.PurchaseOrderItem!)
                    {
                        var inveyory = await _unitOfWork.Repository<InventoryEntity>().GetByConditionAsync(
                            x => x.ProductId == item.ProductId && x.StoreId == storeId);

                        if (inveyory == null)
                            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Product), item.ProductId));

                        inveyory.Quantity -= item.Quantity;
                        await _unitOfWork.Repository<InventoryEntity>().UpdateAsync(inveyory);
                    }
                }
            });
            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(PurchaseOrderEntity)));
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(PurchaseOrderEntity)) + ": " + ex.Message);
        }
    }

    public async Task<OperationResult> UpdateOrderStatusAsync(Guid id, OrderStatusType orderStatus)
    {
        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                var order = await _repository.GetPurchaseOrderWithDetailsByIdAsync(id);
                order.Status = orderStatus;
                if (orderStatus == OrderStatusType.Confirmed)
                {
                    foreach (var item in order.PurchaseOrderItem!)
                    {
                        var inveyory = await _unitOfWork.Repository<InventoryEntity>().GetByConditionAsync(
                            x => x.ProductId == item.ProductId && x.StoreId == order.StoreId);

                        if (inveyory == null)
                            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Product), item.ProductId));

                        inveyory.Quantity -= item.Quantity;
                        await _unitOfWork.Repository<InventoryEntity>().UpdateAsync(inveyory);
                    }
                }
                await _unitOfWork.Repository<PurchaseOrderEntity>().UpdateAsync(order);
            });
            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(PurchaseOrderEntity)));
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(PurchaseOrderEntity) + ": " + ex.Message));
        }
    }
}
