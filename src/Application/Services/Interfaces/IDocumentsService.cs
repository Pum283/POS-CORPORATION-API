namespace Application;

public interface IDocumentsService
{
    Task<QueryResult<Document>> GetDocumentsWithPagingAsync(Guid storeId, QueryInfo queryInfo, QueryDocument queryDate);
    Task<QueryResult<DocumentSupplierForExportrotationModel>> GetDocumentsForExportrotationWithPagingAsync(Guid storeId, QueryInfo queryInfo, DocumentForExportrotationQuery query);
    Task<DocumentSupplierModel> GetDocumentByIdAsync(Guid storeId, Guid id);
    Task<DocumentSupplierForExportrotationModel> GetDocumentForExportrotationByIdAsync(Guid storeId, Guid id);
    Task<OperationResult> CheckCodeExistsAsync(string code);
    Task<OperationResult> GetItemByCodeAsync(string code);
    Task<OperationResult> CreateDocumentAsync(CreateDocumentSupplierModel model, Guid userId);
    Task<OperationResult> CreateDocumentForExportrotationAsync(CreateDocumentSupplierForExportrotationModel model, Guid userId);
    Task<OperationResult> UpdateDocumentStatusAsync(Guid documentId, UpdateDocumentStatusModel model);
}