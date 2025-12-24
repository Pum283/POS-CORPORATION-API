// File: Application/FoodAppPlatformsService.cs
namespace Application;

[RegisterService(typeof(IFoodAppPlatformsService))]
public class FoodAppPlatformsService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : IFoodAppPlatformsService
{
    public async Task<OperationResult> CreateAsync(CreateFoodAppPlatformModel model)
    {
        var exists = await _unitOfWork.Repository<FoodAppPlatformEntity>()
            .GetQueryable()
            .AnyAsync(x => EF.Functions.Collate(x.Name, "SQL_Latin1_General_CP1_CI_AS") == model.Name.Trim());

        if (exists)
            return OperationResult.FailureResult("Tên nền tảng đã tồn tại.");

        var entity = _mapper.Map<FoodAppPlatformEntity>(model);
        entity.Id = Guid.NewGuid();

        await _unitOfWork.Repository<FoodAppPlatformEntity>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return OperationResult.SuccessResult("Tạo nền tảng thành công");
    }

    public async Task<QueryResult<FoodAppPlatformModel>> GetAllWithPagingAsync(QueryInfo queryInfo)
    {
        var query = _unitOfWork.Repository<FoodAppPlatformEntity>()
            .GetQueryable()
            .Where(x => /*x.IsActive == queryInfo.IsActive &&*/
                       (string.IsNullOrEmpty(queryInfo.SearchText) ||
                        EF.Functions.Collate(x.Name, "Latin1_General_CI_AI").Contains(queryInfo.SearchText)))
            .OrderByDescending(x => x.CreateDate);

        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var data = await query
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new FoodAppPlatformModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ShippingType = x.ShippingType,
                IsActive = x.IsActive,
                CreateDate = x.CreateDate
            })
            .ToListAsync();

        return new QueryResult<FoodAppPlatformModel>
        {
            Data = data,
            TotalCount = total
        };
    }

    public async Task<FoodAppPlatformModel> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Repository<FoodAppPlatformEntity>().GetByIdAsync(id)
                     ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Nền tảng", id));

        return _mapper.Map<FoodAppPlatformModel>(entity);
    }

    public async Task<OperationResult> UpdateAsync(Guid id, UpdateFoodAppPlatformModel model)
    {
        var entity = await _unitOfWork.Repository<FoodAppPlatformEntity>().GetByIdAsync(id)
                     ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Nền tảng", id));

        //var nameExists = await _unitOfWork.Repository<FoodAppPlatformEntity>()
        //    .GetQueryable()
        //    .AnyAsync(x => x.Id != id &&
        //                  EF.Functions.Collate(x.Name, "SQL_Latin1_General_CP1_CI_AS") == model.Name.Trim());

        //if (nameExists)
        //    return OperationResult.FailureResult("Tên nền tảng đã được sử dụng bởi nền tảng khác.");

        //_mapper.Map(model, entity);

        if (model.ShippingType.HasValue)
        {
            var exists = await _unitOfWork.Repository<FoodAppPlatformEntity>()
                .GetQueryable()
                .AnyAsync(x => x.Id != id && x.ShippingType == model.ShippingType.Value && x.IsActive);

            if (exists)
                return OperationResult.FailureResult($"ShippingType '{model.ShippingType.Value}' đã được sử dụng bởi nền tảng khác.");
        }

        if (!string.IsNullOrEmpty(model.Description))
            entity.Description = model.Description;

        if (model.ShippingType.HasValue)
            entity.ShippingType = model.ShippingType.Value;
        
        //entity.Description = model.Description;

        await _unitOfWork.SaveChangesAsync();

        return OperationResult.SuccessResult("Cập nhật nền tảng thành công");
    }

    public async Task<OperationResult> ToggleActiveAsync(Guid id)
    {
        var entity = await _unitOfWork.Repository<FoodAppPlatformEntity>().GetByIdAsync(id)
                     ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, "Nền tảng", id));

        entity.IsActive = !entity.IsActive;
        await _unitOfWork.SaveChangesAsync();

        return OperationResult.SuccessResult(
            entity.IsActive
                ? "Kích hoạt nền tảng thành công"
                : "Tắt nền tảng thành công"
        );
    }
}