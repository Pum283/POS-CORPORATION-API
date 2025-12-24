// Application/LoyaltyPointConfigsService.cs
namespace Application;

[RegisterService(typeof(ILoyaltyPointConfigsService))]
public class LoyaltyPointConfigsService(
    IUnitOfWork _unitOfWork,
    IMapper _mapper) : ILoyaltyPointConfigsService
{
    /// <summary>
    /// Tạo cấu hình tích điểm mới
    /// → Tự động tắt tất cả cấu hình cũ đang active
    /// → Chỉ cái mới tạo ra là active (IsActive = true)
    /// </summary>
    public async Task<OperationResult> CreateAsync(CreateLoyaltyPointConfigModel model, Guid createdBy)
    {
        // Lấy tất cả cấu hình cũ đang bật
        var oldActives = await _unitOfWork.Repository<LoyaltyPointConfigEntity>()
            .GetQueryable()
            .Where(x => x.IsActive)
            .ToListAsync();

        // Tắt hết cấu hình cũ (EF sẽ tự track thay đổi, không cần Update/UpdateRange)
        foreach (var item in oldActives)
            item.IsActive = false;

        // Tạo cấu hình mới
        var entity = _mapper.Map<LoyaltyPointConfigEntity>(model);
        entity.Id = Guid.NewGuid();
        entity.CreatedBy = createdBy;      // Người tạo (admin)
        entity.IsActive = true;            // Cái mới luôn là active

        // Thêm vào DB và lưu 1 lần duy nhất (cập nhật cũ + thêm mới)
        await _unitOfWork.Repository<LoyaltyPointConfigEntity>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return OperationResult.SuccessResult("Tạo cấu hình thành công. Chỉ cấu hình mới nhất đang được sử dụng.");
    }

    /// <summary>
    /// Lấy danh sách tất cả cấu hình (dùng cho trang quản trị)
    /// → Active lên đầu, mới nhất lên trước
    /// → Join với User để hiển thị tên người tạo
    /// </summary>
    public async Task<QueryResult<LoyaltyPointConfigModel>> GetAllWithPagingAsync(QueryInfo queryInfo)
    {
        var query = from c in _unitOfWork.Repository<LoyaltyPointConfigEntity>().GetQueryable()
                    join u in _unitOfWork.Repository<UserEntity>().GetQueryable()
                        on c.CreatedBy equals u.Id into gj
                    from creator in gj.DefaultIfEmpty()
                    select new LoyaltyPointConfigModel
                    {
                        Id = c.Id,
                        EarnRate = c.EarnRate,
                        RedeemRate = c.RedeemRate,
                        IsActive = c.IsActive,
                        CreatedByName = creator != null ? creator.Name : "System"
                    };

        var total = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var data = await query
            .OrderByDescending(x => x.IsActive)   // Cái đang active lên đầu
            .ThenByDescending(x => x.Id)          // Mới tạo nhất lên trước
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top > 0 ? queryInfo.Top : 10)
            .ToListAsync();

        return new QueryResult<LoyaltyPointConfigModel> { Data = data, TotalCount = total };
    }

    /// <summary>
    /// Lấy chi tiết 1 cấu hình theo Id
    /// → Dùng khi xem/sửa
    /// </summary>
    public async Task<LoyaltyPointConfigModel> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.Repository<LoyaltyPointConfigEntity>().GetByIdAsync(id)
                     ?? throw new NotFoundException("Không tìm thấy cấu hình.");

        var creator = await _unitOfWork.Repository<UserEntity>().GetByIdAsync(entity.CreatedBy);

        return new LoyaltyPointConfigModel
        {
            Id = entity.Id,
            EarnRate = entity.EarnRate,
            RedeemRate = entity.RedeemRate,
            IsActive = entity.IsActive,
            CreatedByName = creator?.Name ?? "System"
        };
    }

    /// <summary>
    /// Cập nhật tỉ lệ EarnRate / RedeemRate của 1 cấu hình
    /// → Chỉ admin được sửa
    /// </summary>
    public async Task<OperationResult> UpdateAsync(Guid id, UpdateLoyaltyPointConfigModel model)
    {
        var entity = await _unitOfWork.Repository<LoyaltyPointConfigEntity>().GetByIdAsync(id)
                     ?? throw new NotFoundException("Không tìm thấy cấu hình.");

        _mapper.Map(model, entity);
        await _unitOfWork.SaveChangesAsync();

        return OperationResult.SuccessResult("Cập nhật thành công");
    }

    /// <summary>
    /// Bật/tắt thủ công 1 cấu hình (nếu muốn đổi sang cấu hình cũ)
    /// → Khi bật 1 cái thì nên tắt các cái khác ở FE hoặc thêm logic nếu cần
    /// </summary>
    public async Task<OperationResult> ToggleActiveAsync(Guid id)
    {
        var entity = await _unitOfWork.Repository<LoyaltyPointConfigEntity>().GetByIdAsync(id)
                     ?? throw new NotFoundException("Không tìm thấy cấu hình.");

        entity.IsActive = !entity.IsActive;
        await _unitOfWork.SaveChangesAsync();

        return OperationResult.SuccessResult(entity.IsActive ? "Kích hoạt thành công" : "Tắt thành công");
    }

    // Application/LoyaltyPointConfigsService.cs
    /// <summary>
    /// Lấy cấu hình tích điểm đang active (chỉ có 1 cái duy nhất được bật)
    /// Dùng cho toàn bộ hệ thống: tính điểm khi bán hàng, đổi điểm, v.v.
    /// </summary>
    public async Task<LoyaltyPointConfigModel?> GetActiveConfigAsync()
    {
        var entity = await _unitOfWork.Repository<LoyaltyPointConfigEntity>()
            .GetQueryable()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreateDate) // mới nhất nếu có nhiều (phòng trường hợp lỗi dữ liệu)
            .FirstOrDefaultAsync();

        if (entity == null)
            return null;

        var creator = await _unitOfWork.Repository<UserEntity>()
            .GetQueryable()
            .Where(u => u.Id == entity.CreatedBy)
            .Select(u => u.Name)
            .FirstOrDefaultAsync();

        return new LoyaltyPointConfigModel
        {
            Id = entity.Id,
            EarnRate = entity.EarnRate,
            RedeemRate = entity.RedeemRate,
            IsActive = entity.IsActive,
            CreatedByName = creator ?? "System"
        };
    }
}