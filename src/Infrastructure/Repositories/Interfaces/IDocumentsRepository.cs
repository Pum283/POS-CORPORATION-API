namespace Infrastructure;

public interface IDocumentsRepository : IRepositoryBase<DocumentEntity>
{
    Task<QueryResult<Document>> GetDocumentsWithPagingAsync(Guid storeId, QueryInfo queryInfo, QueryDocument queryDate);
    Task<QueryResult<DocumentSupplierForExportrotationModel>> GetDocumentsForExportrotationWithPagingAsync(Guid storeId, QueryInfo queryInfo, DocumentForExportrotationQuery query);
    Task<DocumentSupplierModel?> GetDocumentByIdAsync(Guid storeId, Guid id);
    Task<DocumentSupplierForExportrotationModel> GetDocumentForExportrotationByIdAsync(Guid storeId, Guid id);
}
