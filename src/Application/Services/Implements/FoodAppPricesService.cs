// File: Application/FoodAppPricesService.cs
namespace Application;

[RegisterService(typeof(IFoodAppPricesService))]
public class FoodAppPricesService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IFoodAppPricesService
{
    public async Task<OperationResult> CreateAsync(CreateFoodAppPriceModel model)
    {
        // Kiểm tra tồn tại
        var platform = await _unitOfWork.Repository<FoodAppPlatformEntity>().GetByIdAsync(model.FoodAppPlatformId)
                       ?? throw new NotFoundException("Không tìm thấy nền tảng.");

        var product = await _unitOfWork.Repository<ProductEntity>().GetByIdAsync(model.ProductId)
                      ?? throw new NotFoundException("Không tìm thấy sản phẩm.");

        var exists = await _unitOfWork.Repository<FoodAppPriceEntity>()
            .GetQueryable()
            .AnyAsync(x => x.FoodAppPlatformId == model.FoodAppPlatformId &&
                           x.ProductId == model.ProductId &&
                           x.IsActive);

        if (exists)
            return OperationResult.FailureResult($"Giá của sản phẩm [{product.SKU}] trên [{platform.Name}] đã tồn tại.");

        var entity = _mapper.Map<FoodAppPriceEntity>(model);
        entity.Id = Guid.NewGuid();
        entity.OriginalPrice = product.Price;

        await _unitOfWork.Repository<FoodAppPriceEntity>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return OperationResult.SuccessResult("Tạo giá thành công");
    }

    public async Task<OperationResult> CreateBulkAsync(List<CreateFoodAppPriceModel> models)
    {
        if (models == null || !models.Any()) return OperationResult.FailureResult("Danh sách rỗng.");

        var platformIds = models.Select(x => x.FoodAppPlatformId).Distinct().ToList();
        var productIds = models.Select(x => x.ProductId).Distinct().ToList();

        var platforms = await _unitOfWork.Repository<FoodAppPlatformEntity>()
            .GetQueryable().Where(p => platformIds.Contains(p.Id)).ToListAsync();

        var products = await _unitOfWork.Repository<ProductEntity>()
            .GetQueryable().Where(p => productIds.Contains(p.Id)).ToListAsync();

        var platformDict = platforms.ToDictionary(p => p.Id);
        var productDict = products.ToDictionary(p => p.Id);

        var entities = new List<FoodAppPriceEntity>();
        var errors = new List<string>();

        foreach (var model in models)
        {
            if (!platformDict.ContainsKey(model.FoodAppPlatformId))
            {
                errors.Add($"Nền tảng ID {model.FoodAppPlatformId} không tồn tại");
                continue;
            }

            if (!productDict.ContainsKey(model.ProductId))
            {
                errors.Add($"Sản phẩm ID {model.ProductId} không tồn tại");
                continue;
            }

            var exists = await _unitOfWork.Repository<FoodAppPriceEntity>()
                .GetQueryable()
                .AnyAsync(x => x.FoodAppPlatformId == model.FoodAppPlatformId &&
                               x.ProductId == model.ProductId &&
                               x.IsActive);

            if (exists)
            {
                var p = productDict[model.ProductId];
                var pl = platformDict[model.FoodAppPlatformId];
                errors.Add($"Đã có giá cho [{p.SKU}] trên [{pl.Name}]");
                continue;
            }

            var entity = _mapper.Map<FoodAppPriceEntity>(model);
            entity.Id = Guid.NewGuid();
            entity.OriginalPrice = productDict[model.ProductId].Price;
            entities.Add(entity);
        }

        if (errors.Any())
            return OperationResult.FailureResult(string.Join(" | ", errors));

        if (entities.Any())
        {
            await _unitOfWork.Repository<FoodAppPriceEntity>().AddRangeAsync(entities);
            await _unitOfWork.SaveChangesAsync();
        }

        return OperationResult.SuccessResult($"Tạo thành công {entities.Count} giá");
    }

    public async Task<QueryResult<FoodAppPriceModel>> GetAllWithPagingAsync(QueryInfo queryInfo)
    {
        var query = from price in _unitOfWork.Repository<FoodAppPriceEntity>().GetQueryable()
                    join platform in _unitOfWork.Repository<FoodAppPlatformEntity>().GetQueryable()
                        on price.FoodAppPlatformId equals platform.Id
                    join product in _unitOfWork.Repository<ProductEntity>().GetQueryable()
                        on price.ProductId equals product.Id
                    where /*price.IsActive == queryInfo.IsActive &&*/
                          (string.IsNullOrEmpty(queryInfo.SearchText) ||
                           EF.Functions.Collate(product.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText) ||
                           EF.Functions.Collate(product.SKU, "Latin1_General_CI_AI").Contains(queryInfo.SearchText) ||
                           EF.Functions.Collate(platform.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText))
                    select new FoodAppPriceModel
                    {
                        Id = price.Id,
                        FoodAppPlatformId = price.FoodAppPlatformId,
                        PlatformName = platform.Name,
                        ProductId = price.ProductId,
                        ProductName = product.Name,
                        SKU = product.SKU,
                        OriginalPrice = price.OriginalPrice,
                        FoodAppPrice = price.FoodAppPrice,
                        IsActive = price.IsActive
                    };

        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var data = await query
            .OrderByDescending(x => x.PlatformName)
            .ThenBy(x => x.ProductName)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<FoodAppPriceModel>
        {
            Data = data,
            TotalCount = total
        };
    }

    public async Task<FoodAppPriceModel> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Repository<FoodAppPriceEntity>().GetByIdAsync(id)
                     ?? throw new NotFoundException("Không tìm thấy bảng giá.");

        var platform = await _unitOfWork.Repository<FoodAppPlatformEntity>().GetByIdAsync(entity.FoodAppPlatformId);
        var product = await _unitOfWork.Repository<ProductEntity>().GetByIdAsync(entity.ProductId);

        return new FoodAppPriceModel
        {
            Id = entity.Id,
            FoodAppPlatformId = entity.FoodAppPlatformId,
            PlatformName = platform?.Name ?? "",
            ProductId = entity.ProductId,
            ProductName = product?.Name ?? "",
            SKU = product?.SKU ?? "",
            OriginalPrice = entity.OriginalPrice,
            FoodAppPrice = entity.FoodAppPrice,
            IsActive = entity.IsActive
        };
    }

    public async Task<OperationResult> UpdatePriceAsync(Guid id, UpdateFoodAppPriceModel model)
    {
        var entity = await _unitOfWork.Repository<FoodAppPriceEntity>().GetByIdAsync(id)
                     ?? throw new NotFoundException("Không tìm thấy bảng giá.");

        entity.FoodAppPrice = model.FoodAppPrice;
        await _unitOfWork.SaveChangesAsync();

        return OperationResult.SuccessResult("Cập nhật giá thành công");
    }

    public async Task<OperationResult> ToggleActiveAsync(Guid id)
    {
        var entity = await _unitOfWork.Repository<FoodAppPriceEntity>().GetByIdAsync(id)
                     ?? throw new NotFoundException("Không tìm thấy bảng giá.");

        entity.IsActive = !entity.IsActive;
        await _unitOfWork.SaveChangesAsync();

        return OperationResult.SuccessResult(
            entity.IsActive ? "Kích hoạt giá thành công" : "Tắt giá thành công"
        );
    }

    public async Task<QueryResult<FoodAppPriceModel>> GetByPlatformIdAsync(Guid platformId, QueryInfo queryInfo)
    {
        if (platformId == Guid.Empty)
            throw new ArgumentException("PlatformId không được rỗng");

        var query = from price in _unitOfWork.Repository<FoodAppPriceEntity>().GetQueryable()
                    join platform in _unitOfWork.Repository<FoodAppPlatformEntity>().GetQueryable()
                        on price.FoodAppPlatformId equals platform.Id
                    join product in _unitOfWork.Repository<ProductEntity>().GetQueryable()
                        on price.ProductId equals product.Id
                    where price.FoodAppPlatformId == platformId
                    select new FoodAppPriceModel
                    {
                        Id = price.Id,
                        FoodAppPlatformId = price.FoodAppPlatformId,
                        PlatformName = platform.Name,
                        ProductId = price.ProductId,
                        ProductName = product.Name,
                        SKU = product.SKU,
                        OriginalPrice = price.OriginalPrice,
                        FoodAppPrice = price.FoodAppPrice,
                        IsActive = price.IsActive
                    };

        // Tìm kiếm theo tên sản phẩm hoặc SKU (rất hay dùng khi xem 1 platform)
        if (!string.IsNullOrWhiteSpace(queryInfo.SearchText))
        {
            var search = queryInfo.SearchText.Trim();
            query = query.Where(x =>
                EF.Functions.Collate(x.ProductName, "Latin1_General_CI_AI").Contains(search) ||
                EF.Functions.Collate(x.SKU, "Latin1_General_CI_AI").Contains(search));
        }

        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        // Sắp xếp đẹp: tên sản phẩm A → Z
        var data = await query
            .OrderBy(x => x.ProductName)
            .ThenBy(x => x.SKU)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top > 0 ? queryInfo.Top : 10)  // mặc định 10 nếu không truyền top
            .ToListAsync();

        return new QueryResult<FoodAppPriceModel>
        {
            Data = data,
            TotalCount = total
        };
    }
}