namespace API.Controllers;

[Route("documents")]
[Authorize]
public class DocumentsController(
    IDocumentsService _service,
    IMapper _mapper,
    IHubContext<DocumentHub> _hubContext) : BaseApiController
{
    [HttpGet]
    public async Task<QueryResult<Document>> GetDocumentsWithPaging([FromQuery] QueryInfo queryInfo, [FromQuery] QueryDocument queryDate)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetDocumentsWithPagingAsync(storeId, queryInfo, queryDate);
    }
    [HttpGet("exportrotation")]
    [ValidateModel]
    public async Task<QueryResult<DocumentSupplierForExportrotationModel>> GetDocumentsForExportrotationWithPaging([FromQuery] QueryInfo queryInfo, [FromQuery] DocumentForExportrotationQuery documentForExportrotationQuery)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetDocumentsForExportrotationWithPagingAsync(storeId, queryInfo, documentForExportrotationQuery);
    }
    [HttpGet("{storeId}/store/accountant")]
    public async Task<QueryResult<Document>> GetDocumentsForAccountantWithPaging([FromRoute] Guid storeId, [FromQuery] QueryInfo queryInfo, [FromQuery] QueryDocument queryDate)
    {
        return await _service.GetDocumentsWithPagingAsync(storeId, queryInfo, queryDate);
    }

    [HttpGet("{id}")]
    public async Task<DocumentSupplierModel> GetDocumentById(Guid id)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetDocumentByIdAsync(storeId, id);
    }

    [HttpGet("exportrotation/{id}")]
    public async Task<DocumentSupplierForExportrotationModel> GetDocumentForExportrotationById(Guid id)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        return await _service.GetDocumentForExportrotationByIdAsync(storeId, id);
    }

    [HttpGet("checkCodeExists/{code}")]
    public async Task<OperationResult> CheckCodeExists(string code)
    {
        return await _service.CheckCodeExistsAsync(code);
    }

    [HttpGet("GetItemByCode/{code}")]
    public async Task<OperationResult> GetItemByCode(string code)
    {
        return await _service.GetItemByCodeAsync(code);
    }

    [HttpPost("supplier")]
    [ValidateModel]
    public async Task<OperationResult> CreateDocument([FromBody] CreateDocumentSupplierModel model)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        model.StoreId = storeId;
        var result = await _service.CreateDocumentAsync(model, userId);
        return result;
    }

    [HttpPost("supplier/exportrotation")]
    [ValidateModel]
    public async Task<OperationResult> CreateDocumentForExportrotation([FromBody] CreateDocumentSupplierForExportrotationModel model)
    {
        var storeId = HttpContextHelper.GetCurrentStoreId(HttpContext);
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);

        model.FromStoreId = storeId;
        return await _service.CreateDocumentForExportrotationAsync(model, userId);
    }
    [HttpPatch("update-status-exportrotation/{documentId}/document")]
    [ValidateModel]
    public async Task<OperationResult> UpdateDocumentStatus([FromRoute] Guid documentId, [FromBody] UpdateDocumentStatusModel model)
    {
        var result = await _service.UpdateDocumentStatusAsync(documentId, model);
        return result;
    }

    // ACCOUNTANT
    [HttpPost("supplier/accountant")]
    [ValidateModel]
    public async Task<OperationResult> CreateDocumentForAccountant([FromBody] CreateDocumentSupplierForAccountantModel model)
    {
        var document = _mapper.Map<CreateDocumentSupplierModel>(model);
        var userId = HttpContextHelper.GetCurrentUserId(HttpContext);

        var result = await _service.CreateDocumentAsync(document, userId);

        if (result.Success && result.Data is DocumentCreatedData data)
        {
            await _hubContext.Clients.Groups($"store-{model.StoreId}")
                .SendAsync("DocumentDataChanged", new { action = "create", productIds = data.ProductIds });
        }

        return result;
    }


}