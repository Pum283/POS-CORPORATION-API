namespace Application;

public interface IMocksService
{
    Task<OperationResult> CreateMockDocumentAsync(List<MockUpdateInventoryModel> model, Guid storeId);
}
