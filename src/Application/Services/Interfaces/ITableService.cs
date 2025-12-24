namespace Application;

public interface ITableService
{
    Task<TableResponse> CreateAsync(TableCreateRequest request);
    Task<TableResponse?> GetByIdAsync(Guid id);
    Task<List<TableResponse>> GetAllAsync(Guid? storeId = null);
    Task<bool> UpdateAsync(TableUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
}