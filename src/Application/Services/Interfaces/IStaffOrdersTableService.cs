
namespace Application
{
    public interface IStaffOrdersTableService
    {
        Task AddOrUpdateItemAsync(Guid staffId, Guid tableId, Guid productId, int quantity, Guid storeId, string? note = null);
        Task<List<StaffOrdersTableItem>> GetItemsAsync(Guid storeId, Guid tableId);
        Task RemoveItemAsync(Guid storeId, Guid tableId, Guid productId);
        Task ClearAllAsync(Guid storeId, Guid tableId);
        Task AddOrUpdateMultipleItemsAsync(Guid staffId, Guid tableId, List<AddItemDetail> items, Guid storeId);
        Task<ReduceResult> ReduceQuantityAsync(Guid storeId, Guid tableId, string productIdString, int reduceBy);
    }
}