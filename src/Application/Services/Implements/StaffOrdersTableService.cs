using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application;

[RegisterService(typeof(IStaffOrdersTableService))]
public class StaffOrdersTableService(
    IRedisService redisService,
    IProductsRepository productRepository,
    IHttpContextAccessor httpContextAccessor) : IStaffOrdersTableService
{
    private readonly IRedisService _redisService = redisService;
    private readonly IProductsRepository _productRepository = productRepository;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(48);

    private Guid CurrentStoreId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null || context.User?.Identity?.IsAuthenticated != true)
                return Guid.Empty;

            var claim = context.User.FindFirst("storeId") ?? context.User.FindFirst("StoreId");
            return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
        }
    }

    private string GetKey(Guid staffId) => $"staff:order:table:{staffId}";

    private string GetKey(Guid storeId, Guid tableId)
        => $"staff:order:table:store:{storeId}:table:{tableId}";

    public async Task AddOrUpdateItemAsync(Guid staffId, Guid tableId, Guid productId, int quantity, Guid storeId, string? note = null)
    {
        await AddOrUpdateMultipleItemsAsync(staffId, tableId, new List<AddItemDetail>
        {
            new() { ProductId = productId, Quantity = quantity, Note = note }
        }, storeId);
    }

    public async Task<List<StaffOrdersTableItem>> GetItemsAsync(Guid storeId, Guid tableId)
    {
        var key = GetKey(storeId, tableId);
        var items = await _redisService.GetAsync<List<StaffOrdersTableItem>?>(key);
        return items ??= new List<StaffOrdersTableItem>();
    }

    public async Task RemoveItemAsync(Guid storeId, Guid tableId, Guid productId)
    {
        var key = GetKey(storeId, tableId);
        var items = await _redisService.GetAsync<List<StaffOrdersTableItem>?>(key);
        if (items == null || items.Count == 0) return;

        items.RemoveAll(x => x.ProductId == productId);

        if (items.Count > 0)
            await _redisService.SetAsync(key, items, CacheDuration);
        else
            await _redisService.DeleteAsync(key);
    }

    public async Task ClearAllAsync(Guid storeId, Guid tableId)
    {
        var key = GetKey(storeId, tableId);
        await _redisService.DeleteAsync(key);
    }

    public async Task AddOrUpdateMultipleItemsAsync(
    Guid staffId,
    Guid tableId,
    List<AddItemDetail> items,
    Guid storeId)
    {
        if (tableId == Guid.Empty)
            throw new ArgumentException("Vui lòng chọn bàn");

        if (items == null || items.Count == 0)
            throw new ArgumentException("Danh sách món trống");

        var key = GetKey(storeId, tableId);
        var list = await _redisService.GetAsync<List<StaffOrdersTableItem>?>(key)
                   ?? new List<StaffOrdersTableItem>();

        // Dùng ToDictionary với null-forgiving nếu chắc chắn không trùng ProductId
        var existingDict = list.ToDictionary(x => x.ProductId, x => x);

        foreach (var (detail, index) in items.Select((d, i) => (d, i)))
        {
            if (detail == null)
                throw new ArgumentException($"Item thứ {index + 1} bị null");

            if (detail.ProductId == Guid.Empty)
                throw new ArgumentException($"Item thứ {index + 1}: ProductId trống");

            if (detail.Quantity == 0)
                throw new ArgumentException($"Item thứ {index + 1}: Số lượng không được bằng 0");
        }

        foreach (var detail in items)
        {
            var product = await _productRepository.GetProductForStaffOrderAsync(detail.ProductId, storeId)
                          ?? throw new KeyNotFoundException($"Không tìm thấy sản phẩm {detail.ProductId}");

            if (!product.IsActive)
                throw new InvalidOperationException($"Sản phẩm '{product.Name}' đã ngừng kinh doanh");

            int currentQuantity = 0;
            StaffOrdersTableItem? existingItem = null;

            if (existingDict.TryGetValue(detail.ProductId, out var item))
            {
                existingItem = item;
                currentQuantity = item.Quantity;
            }

            int newQuantity = currentQuantity + detail.Quantity;

            // Bắt buộc số lượng cuối cùng phải >= 1
            if (newQuantity <= 0)
            {
                throw new InvalidOperationException(
                    $"Không thể điều chỉnh món '{product.Name}'. " +
                    $"Hiện tại: {currentQuantity}, yêu cầu thay đổi: {detail.Quantity:+#;-#;0} → còn {newQuantity}. " +
                    $"Số lượng phải lớn hơn 0.");
            }

            // Kiểm tra tồn kho nếu tăng số lượng
            if (newQuantity > currentQuantity && product.Quantity < newQuantity)
            {
                int needMore = newQuantity - currentQuantity;
                throw new InvalidOperationException(
                    $"Chỉ còn {product.Quantity} {product.Unit} '{product.Name}', không đủ để thêm {needMore}.");
            }

            if (existingItem != null)
            {
                // Cập nhật món đã có
                existingItem.Quantity = newQuantity;
                existingItem.Note = detail.Note;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Thêm mới (chỉ xảy ra khi quantity > 0 → newQuantity >= 1)
                var newItem = new StaffOrdersTableItem
                {
                    ProductId = detail.ProductId,
                    Quantity = newQuantity,
                    Note = detail.Note,
                    UpdatedAt = DateTime.UtcNow,
                    StoreId = storeId,
                    StaffId = staffId
                };
                list.Add(newItem);
                existingDict[detail.ProductId] = newItem; // cập nhật dict
            }
        }

        await _redisService.SetAsync(key, list, CacheDuration);
    }

    public async Task<ReduceResult> ReduceQuantityAsync(Guid storeId, Guid tableId, string productIdString, int reduceBy)
    {
        if (tableId == Guid.Empty)
            throw new ArgumentException("Bàn không được để trống");

        if (string.IsNullOrWhiteSpace(productIdString))
            throw new ArgumentException("ProductId không được để trống");

        if (!Guid.TryParse(productIdString, out var productId) || productId == Guid.Empty)
            throw new ArgumentException("ProductId không hợp lệ");

        if (reduceBy <= 0)
            throw new ArgumentException("Số lượng giảm phải lớn hơn 0");

        var key = GetKey(storeId, tableId);
        var items = await _redisService.GetAsync<List<StaffOrdersTableItem>?>(key)
                    ?? new List<StaffOrdersTableItem>();

        var item = items.FirstOrDefault(x => x.ProductId == productId);
        if (item == null)
            throw new KeyNotFoundException($"Không tìm thấy món {productId} trong bàn {tableId}");

        item.Quantity -= reduceBy;
        item.UpdatedAt = DateTime.UtcNow;

        if (item.Quantity <= 0)
        {
            items.Remove(item);

            if (items.Count == 0)
                await _redisService.DeleteAsync(key);
            else
                await _redisService.SetAsync(key, items, CacheDuration);

            return new ReduceResult { RemainingQuantity = 0, WasRemoved = true };
        }

        await _redisService.SetAsync(key, items, CacheDuration);
        return new ReduceResult { RemainingQuantity = item.Quantity, WasRemoved = false };
    }

    public async Task ClearAllAsync(Guid staffId)
        => await _redisService.DeleteAsync(GetKey(staffId));
}
