namespace Infrastructure;

[RegisterService(typeof(IDocumentsRepository))]
public class DocumentsRepository(EfApplicationDbContext context) : RepositoryBase<DocumentEntity>(context), IDocumentsRepository
{
    private readonly EfApplicationDbContext _context = context;
    #region  GetDocumentsWithPagingAsync
    //public async Task<QueryResult<Document>> GetDocumentsWithPagingAsync(Guid storeId, QueryInfo queryInfo, QueryDocument queryDate)
    //{
    //    var query = from document in _context.Documents.AsNoTracking()
    //                where _context.DocumentSuppliers
    //                          .Any(ds => ds.DocumentId == document.Id &&
    //                                     ds.TransType == queryDate.TransType &&
    //                                    (storeId == Guid.Empty || ds.StoreId == storeId))
    //                      && (!queryDate.From.HasValue || document.CreateDate >= queryDate.From.Value)
    //                      && (!queryDate.To.HasValue || document.CreateDate <= queryDate.To.Value)

    //                orderby document.CreateDate descending
    //                select new Document
    //                {
    //                    Id = document.Id,
    //                    DocumentName = document.DocumentName,
    //                    Status = document.Status.ToString(),
    //                    CreateDate = document.CreateDate.ToString("dd-MM-yyyy HH:mm")
    //                };

    //    var pagedEntities = await query
    //        .Skip(queryInfo.Skip)
    //        .Take(queryInfo.Top)
    //        .ToListAsync();

    //    int totalCount = 0;
    //    if (queryInfo.NeedTotalCount)
    //    {
    //        totalCount = await query.CountAsync();
    //    }

    //    return new QueryResult<Document>
    //    {
    //        Data = pagedEntities,
    //        TotalCount = totalCount
    //    };
    //}
    #endregion
    public async Task<QueryResult<Document>> GetDocumentsWithPagingAsync(
    Guid storeId,
    QueryInfo queryInfo,
    QueryDocument queryDate)
    {
        var fromDate = queryDate.From?.Date;
        var toDate = queryDate.To?.Date.AddDays(1); // FIX: tăng 1 ngày

        var query = from document in _context.Documents.AsNoTracking()
                    join user in _context.Users.AsNoTracking() on document.UserId equals user.Id into userGroup
                    from user in userGroup.DefaultIfEmpty()
                    where _context.DocumentSuppliers
                              .Any(ds => ds.DocumentId == document.Id &&
                                         ds.TransType == queryDate.TransType &&
                                        (storeId == Guid.Empty || ds.StoreId == storeId))
                          && (!fromDate.HasValue || document.CreateDate >= fromDate.Value)
                          && (!toDate.HasValue || document.CreateDate < toDate.Value)
                    orderby document.CreateDate descending
                    select new Document
                    {
                        Id = document.Id,
                        DocumentName = document.DocumentName,
                        Status = document.Status.ToString(),
                        CreateDate = document.CreateDate.ToString("dd-MM-yyyy HH:mm"),
                        UserCreateId = document.UserId,
                        UserCreateName = user != null ? user.Name : "Chưa đặt tên"
                    };

        var pagedEntities = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        int totalCount = queryInfo.NeedTotalCount
            ? await query.CountAsync()
            : 0;

        return new QueryResult<Document>
        {
            Data = pagedEntities,
            TotalCount = totalCount
        };
    }
    public async Task<QueryResult<DocumentSupplierForExportrotationModel>> GetDocumentsForExportrotationWithPagingAsync(
    Guid storeId,
    QueryInfo queryInfo,
    DocumentForExportrotationQuery filter)
    {
        var fromDate = filter.FromDate.Date;
        var toDate = filter.ToDate.Date.AddDays(1);
        var baseQuery = from document in _context.Documents.AsNoTracking()
                        join documentSupplier in _context.DocumentSuppliers.AsNoTracking()
                            on document.Id equals documentSupplier.DocumentId
                        join fromStore in _context.Stores.AsNoTracking()
                            on documentSupplier.StoreId equals fromStore.Id
                        
                        where documentSupplier.TransType == TransType.Exportrotation &&
                              document.CreateDate >= fromDate &&
                              document.CreateDate < toDate &&
                              (storeId == Guid.Empty || documentSupplier.ToStoreId == storeId || documentSupplier.StoreId == storeId)
                        where !filter.ProductId.HasValue && !filter.IngredientId.HasValue ||
                              _context.DocumentSuppliers.Any(ds =>
                              ds.DocumentId == document.Id &&
                              ((filter.ProductId.HasValue && ds.ProductId == filter.ProductId.Value) ||
                              (filter.IngredientId.HasValue && ds.IngredientId == filter.IngredientId.Value))
                              )
                        orderby document.CreateDate descending
                        select new
                        {
                            DocumentId = document.Id,
                            FromStoreId = documentSupplier.StoreId,
                            FromStoreName = fromStore.Name,
                            ToStoreId = documentSupplier.ToStoreId ?? Guid.Empty,
                            Description = document.Description,
                            DocumentStatus = document.Status
                        };

        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.DocumentStatusType) &&
                Enum.TryParse<DocumentStatusType>(filter.DocumentStatusType, out var status))
            {
                baseQuery = baseQuery.Where(x => x.DocumentStatus == status);
            }
        }

        var pagedData = await baseQuery
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        var entities = pagedData.Select(x => new DocumentSupplierForExportrotationModel
        {
            DocumentId = x.DocumentId,
            FromStoreId = x.FromStoreId,
            FromStoreName = x.FromStoreName,
            ToStoreId = x.ToStoreId,
            Description = x.Description,
            DocumentStatusType = x.DocumentStatus.ToString()
        }).ToList();

        int totalCount = 0;
        if (queryInfo.NeedTotalCount)
        {
            totalCount = await baseQuery.CountAsync();
        }

        return new QueryResult<DocumentSupplierForExportrotationModel>
        {
            Data = entities,
            TotalCount = totalCount
        };
    }

    public async Task<DocumentSupplierModel?> GetDocumentByIdAsync(Guid storeId, Guid id)
    {
        if (storeId == Guid.Empty)
            return null;

        var result = await (
            from documentSupplier in _context.DocumentSuppliers.AsNoTracking()
            join document in _context.Documents.AsNoTracking() on documentSupplier.DocumentId equals document.Id
            join store in _context.Stores.AsNoTracking() on documentSupplier.StoreId equals store.Id
            join toStoreTmp in _context.Stores.AsNoTracking()
                    on documentSupplier.ToStoreId equals toStoreTmp.Id into toStoreJoin
            from toStore in toStoreJoin.DefaultIfEmpty()
                //where documentSupplier.DocumentId == id &&
                //      (storeId == Guid.Empty || storeId == documentSupplier.StoreId)
            where documentSupplier.DocumentId == id &&
              (documentSupplier.StoreId == storeId || documentSupplier.ToStoreId == storeId)
            //join ingredient in _context.Ingredients.AsNoTracking() on documentSupplier.IngredientId equals ingredient.Id into ingredientGroup
            select new DocumentSupplierModel
            {
                Id = document.Id,
                StoreId = documentSupplier.StoreId,
                StoreName = store.Name,
                ToStoreId = documentSupplier.ToStoreId,
                //ToStoreName = toStore.Name,
                ToStoreName = toStore != null ? toStore.Name : null,
                TransType = documentSupplier.TransType.ToString(),
                Description = document.Description,
                Document = new Document
                {
                    Id = document.Id,
                    DocumentName = document.DocumentName,
                    Status = document.Status.ToString(),
                    CreateDate = document.CreateDate.ToString("dd-MM-yyyy HH:mm")
                }
            }
        ).FirstOrDefaultAsync();

        if (result == null) return null;

        result.Products = await (
            from dp in _context.DocumentSuppliers.AsNoTracking()
            join p in _context.Products.AsNoTracking() on dp.ProductId equals p.Id
            where dp.DocumentId == result.Document!.Id
            select new Product
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Price = p.Price,
                Unit = p.Unit.ToString(),
                Quantity = dp.Quantity,
                Cost = p.Cost,
                VAT = p.VAT
            }
        ).ToListAsync();

        result.Ingredients = await (
            from dp in _context.DocumentSuppliers.AsNoTracking()
            join i in _context.Ingredients.AsNoTracking() on dp.IngredientId equals i.Id
            where dp.DocumentId == result.Document!.Id
            select new Ingredient
            {
                Id = i.Id,
                Name = i.Name,
                Code = i.Code,
                Unit = i.Unit.ToString(),
                Quantity = dp.Quantity,
            }
        ).ToListAsync();

        return result;
    }

    public Task<DocumentSupplierForExportrotationModel> GetDocumentForExportrotationByIdAsync(Guid storeId, Guid id)
    {
        throw new NotImplementedException();
    }
}
