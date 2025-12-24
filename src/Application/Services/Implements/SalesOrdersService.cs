using Domain;

namespace Application;

[RegisterService(typeof(ISalesOrdersService))]
public class SalesOrdersService
    (

    ISalesOrdersRepository _repository,
    IStoresRepository _storeRepository,
    IProductsRepository _productsRepository,
    IVietQRPaymentService _vietQRPaymentService,
    IProductIngredientsService _productIngredientsService,
    IUnitOfWork _unitOfWork) : ISalesOrdersService
    //,IMapper _mapper
{
    public async Task<QueryResult<SalesOrderOfCustomerModel>> GetSalesOrdersOfCustomerWithPagingAsync(Guid customerId, QueryInfo queryInfo)
    {
        return await _repository.GetSalesOrdersOfCustomerWithPagingAsync(customerId, queryInfo);
    }
    public async Task<QueryResult<SalesOrderModel>> GetSalesOrdersWithPaging(Guid storeId, QueryInfo queryInfo, SalesOrderQuery orderQuery)
    {
        return await _repository.GetSalesOrdersWithPagingAsync(storeId, queryInfo, orderQuery);
    }
    public async Task<SalesOrderModel> GetSalesOrderByIdAsync(Guid id)
    {
        return await _repository.GetSalesOrderByIdAsync(id);
    }
    //public async Task<OperationResult> CreateAsync(CreateSalesOrderModel model)
    //{
    //    try
    //    {
    //        object? result = null;
    //        var deductIngredient = new List<DeductIngredientModel>();
    //        await _unitOfWork.ExecuteWithTransactionAsync(async () =>
    //        {
    //            var salesOrder = _mapper.Map<SalesOrderEntity>(model);

    //            var store = await _storeRepository.GetByIdAsync(model.StoreId);
    //            if (store == null)
    //                throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Store), model.StoreId));

    //            salesOrder.Id = Guid.NewGuid();
    //            salesOrder.OrderCode = TextHelper.GenerateOrderCode(store.Name);

    //            foreach (var item in salesOrder.SalesOrderItem!)
    //            {
    //                var product = await _productsRepository.GetProductForSalesOrderAsync(item.ProductId);
    //                item.UnitPrice = product.Price;
    //                item.TotalPrice = item.UnitPrice * item.Quantity;
    //            }
    //            salesOrder.TotalAmount = salesOrder.SalesOrderItem!.Sum(item => item.TotalPrice);

    //            salesOrder = await ApplyPromotion(model.SalesOrderPromotion, salesOrder);

    //            if (salesOrder.PaymentMethod == PaymentMethodType.Online)
    //            {
    //                var bank = (await _unitOfWork.Repository<BankEntity>().GetAllAsync()).FirstOrDefault();
    //                if (bank == null)
    //                    throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(BankEntity)));
    //                // Online payment
    //                salesOrder.OrderStatus = OrderStatusType.Pending;
    //                salesOrder.PaidAmount = ApplyLoyaltyPoints(model.CustomerId ?? Guid.Empty, salesOrder.TotalAmount, salesOrder.OrderStatus, model.IsUsingPoints);

    //                result = await _unitOfWork.Repository<SalesOrderEntity>().AddAsync(salesOrder);
    //                var apiRequest = new ApiRequest
    //                {
    //                    acqId = bank.acqId,
    //                    accountNo = bank.accountNo,
    //                    accountName = bank.accountName,
    //                    amount = (int)Math.Round(salesOrder.PaidAmount),
    //                    format = bank.format,
    //                    template = bank.template,
    //                };
    //                var imageBytes = _vietQRPaymentService.GetVietQRImage(apiRequest);

    //                var base64Image = Convert.ToBase64String(imageBytes);
    //                result = new
    //                {
    //                    orderId = salesOrder.Id,
    //                    qr = $"data:image/png;base64,{base64Image}"
    //                };
    //            }
    //            else
    //            {
    //                // COD (Cash on Delivery)
    //                var productIds = model.SalesOrderItem!.Select(x => x.ProductId).Distinct().ToList();
    //                var inventories = await _unitOfWork.Repository<InventoryEntity>().GetQueryable()
    //                    .Where(x => x.StoreId == model.StoreId && productIds.Contains(x.ProductId))
    //                    .ToListAsync();

    //                foreach (var item in model.SalesOrderItem!)
    //                {
    //                    var inventory = inventories.FirstOrDefault(x => x.ProductId == item.ProductId);
    //                    if (inventory == null)
    //                        throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(InventoryEntity), item.ProductId));

    //                    inventory.Quantity -= item.Quantity;

    //                    deductIngredient.Add(new DeductIngredientModel
    //                    {
    //                        StoreId = model.StoreId,
    //                        ProductId = item.ProductId,
    //                        Quantity = item.Quantity
    //                    });
    //                }

    //                foreach (var item in inventories)
    //                {
    //                    _unitOfWork.Repository<InventoryEntity>().UpdateWithoutSave(item);
    //                }

    //                salesOrder.OrderStatus = OrderStatusType.Confirmed;
    //                salesOrder.PaidAmount = ApplyLoyaltyPoints(model.CustomerId ?? Guid.Empty, salesOrder.TotalAmount, salesOrder.OrderStatus, model.IsUsingPoints);

    //                await _unitOfWork.Repository<SalesOrderEntity>().AddAsync(salesOrder);
    //                // deduct ingredients in inventory
    //                await _productIngredientsService.DeductIngredientAsync(deductIngredient);
    //                result = new
    //                {
    //                    orderId = salesOrder.Id,
    //                    isUsingPoints = model.IsUsingPoints
    //                };
    //            }
    //        });
    //        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(SalesOrder)),
    //        result);
    //    }
    //    catch (Exception ex)
    //    {
    //        return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(SalesOrder)) + ": " + ex.InnerException?.Message);
    //    }
    //}
    //public async Task<OperationResult> CreateAsync(CreateSalesOrderModel model)
    //{
    //    try
    //    {
    //        object? result = null;
    //        SalesOrderEntity salesOrder = null!;
    //        // EF Core execution strategy
    //        var strategy = _unitOfWork.CreateExecutionStrategyAsync();

    //        await strategy.ExecuteAsync(async () =>
    //        {
    //            // Bọc toàn bộ trong transaction
    //            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
    //            {
    //                salesOrder = _mapper.Map<SalesOrderEntity>(model);

    //                // --- Validate & chuẩn bị dữ liệu ---
    //                var store = await _storeRepository.GetByIdAsync(model.StoreId);

    //                salesOrder.Id = Guid.NewGuid();
    //                salesOrder.OrderCode = TextHelper.GenerateOrderCode(store.Name);

    //                foreach (var item in salesOrder.SalesOrderItem!)
    //                {
    //                    var product = await _productsRepository.GetProductForSalesOrderAsync(item.ProductId); // có thể get bên ngoài for để tối ưu performance
    //                    item.UnitPrice = product.Price;
    //                    item.TotalPrice = item.UnitPrice * item.Quantity;
    //                }
    //                salesOrder.TotalAmount = salesOrder.SalesOrderItem!.Sum(x => x.TotalPrice);

    //                salesOrder = await ApplyPromotion(model.SalesOrderPromotion, salesOrder);

    //                // --- Branch: Online payment ---
    //                if (salesOrder.PaymentMethod == PaymentMethodType.Online)
    //                {
    //                    var bank = (await _unitOfWork.Repository<BankEntity>().GetAllAsync()).FirstOrDefault()
    //                        ?? throw new NotFoundException("Bank not found");

    //                    salesOrder.OrderStatus = OrderStatusType.Pending;
    //                    salesOrder.PaidAmount = ApplyLoyaltyPoints(
    //                        model.CustomerId ?? Guid.Empty,
    //                        salesOrder.TotalAmount,
    //                        salesOrder.OrderStatus,
    //                        model.IsUsingPoints
    //                    );

    //                    await _unitOfWork.Repository<SalesOrderEntity>().AddAsync(salesOrder);

    //                    // ❗ Không gọi API ngoài DB ở đây
    //                    result = new { orderId = salesOrder.Id };
    //                }
    //                else
    //                {
    //                    // --- Branch: COD ---
    //                    var productIds = model.SalesOrderItem!.Select(x => x.ProductId).Distinct().ToList();

    //                    // Lấy inventory trong transaction
    //                    var inventories = await _unitOfWork.Repository<InventoryEntity>().GetQueryable()
    //                        .Where(x => x.StoreId == model.StoreId && productIds.Contains(x.ProductId))
    //                        .ToListAsync();

    //                    foreach (var item in model.SalesOrderItem!)
    //                    {
    //                        var inventory = inventories.FirstOrDefault(x => x.ProductId == item.ProductId)
    //                            ?? throw new NotFoundException($"Inventory for product {item.ProductId} not found");

    //                        // Trừ tồn kho
    //                        inventory.Quantity -= item.Quantity;

    //                        _unitOfWork.Repository<InventoryEntity>().UpdateWithoutSave(inventory);
    //                    }

    //                    salesOrder.OrderStatus = OrderStatusType.Confirmed;
    //                    salesOrder.PaidAmount = ApplyLoyaltyPoints(
    //                        model.CustomerId ?? Guid.Empty,
    //                        salesOrder.TotalAmount,
    //                        salesOrder.OrderStatus,
    //                        model.IsUsingPoints
    //                    );

    //                    await _unitOfWork.Repository<SalesOrderEntity>().AddAsync(salesOrder);
    //                    await _productIngredientsService.DeductIngredientAsync(
    //                        model.SalesOrderItem!.Select(i => new DeductIngredientModel
    //                        {
    //                            StoreId = model.StoreId,
    //                            ProductId = i.ProductId,
    //                            Quantity = i.Quantity
    //                        }).ToList()
    //                    );

    //                    result = new { orderId = salesOrder.Id, isUsingPoints = model.IsUsingPoints };
    //                }
    //            });

    //            // ❗ API call sau khi transaction commit
    //            if (salesOrder.PaymentMethod == PaymentMethodType.Online)
    //            {
    //                var bank = (await _unitOfWork.Repository<BankEntity>().GetAllAsync()).FirstOrDefault();
    //                var apiRequest = new ApiRequest
    //                {
    //                    acqId = bank!.acqId,
    //                    accountNo = bank.accountNo,
    //                    accountName = bank.accountName,
    //                    amount = (int)Math.Round(salesOrder.PaidAmount),
    //                    format = bank.format,
    //                    template = bank.template
    //                };
    //                var imageBytes = _vietQRPaymentService.GetVietQRImage(apiRequest);
    //                var base64Image = Convert.ToBase64String(imageBytes);

    //                result = new { orderId = salesOrder.Id, qr = $"data:image/png;base64,{base64Image}" };
    //            }
    //        });

    //        return OperationResult.SuccessResult($"SalesOrder created successfully", result);
    //    }
    //    catch (Exception ex)
    //    {
    //        return OperationResult.FailureResult("Failed to create order: " + ex.Message);
    //    }
    //}

    public async Task<OperationResult> CreateAsync(CreateSalesOrderRequest request, Guid userId)
    {
        try
        {
            object? result = null;
            SalesOrderEntity salesOrder = null!;

            // DỜI LÊN ĐẦU ĐỂ DÙNG ĐƯỢC Ở NHIỀU NƠI
            var paymentMethod = Enum.Parse<PaymentMethodType>(request.PaymentMethod, true);

            var strategy = _unitOfWork.CreateExecutionStrategyAsync();
            await strategy.ExecuteAsync(async () =>
            {
                await _unitOfWork.ExecuteWithTransactionAsync(async () =>
                {
                    // === TOÀN BỘ CODE TẠO ĐƠN, TÍNH GIÁ, TRỪ KHO (COD), THÊM PROMOTION ===
                    // (giữ nguyên từ var store = ... đến hết phần COD)

                    var store = await _storeRepository.GetByIdAsync(request.StoreId)
                        ?? throw new NotFoundException("Không tìm thấy cửa hàng");

                    salesOrder = new SalesOrderEntity
                    {
                        Id = Guid.NewGuid(),
                        StoreId = request.StoreId,
                        CustomerId = request.CustomerId,
                        OrderCode = TextHelper.GenerateOrderCode(store.Name),
                        UserId = userId,
                        PaymentMethod = paymentMethod,
                        OrderStatus = paymentMethod == PaymentMethodType.Online   
                            ? OrderStatusType.Pending
                            : OrderStatusType.Confirmed,
                        SalesOrderItem = new List<SalesOrderItemEntity>()
                    };

                    var shippingType = Enum.Parse<ShippingType>(request.ShippingType, ignoreCase: true);
                    // Chuyển string từ FE ("GrabFood", "ShopeeFood", "None", "1", "2"...) → enum ShippingType
                    // ignoreCase = true → chấp nhận cả chữ hoa/thường
                    salesOrder.ShippingType = shippingType;
                    // Lưu lại nguồn đơn hàng (để sau này báo cáo, lọc đơn Grab/ShopeeFood...)

                    // ================================================
                    // XỬ LÝ ĐƠN TỪ CÁC NỀN TẢNG GIAO HÀNG (GrabFood, ShopeeFood, Be, LoShip...)
                    // → BẮT BUỘC dùng giá riêng đã cấu hình trong bảng FoodAppPrices
                    // → KHÔNG áp dụng khuyến mãi, KHÔNG dùng điểm tích lũy
                    // → TotalAmount = PaidAmount (không giảm gì cả)
                    // ================================================
                    if (shippingType != ShippingType.None)
                    {
                        // 1. Tìm nền tảng (GrabFood, ShopeeFood...) theo ShippingType
                        //    + BẮT BUỘC phải IsActive = true, nếu tắt thì báo lỗi ngay
                        var platform = await _unitOfWork.Repository<FoodAppPlatformEntity>()
                            .GetQueryable()
                            .FirstOrDefaultAsync(p => p.ShippingType == shippingType && p.IsActive)
                            ?? throw new ParameterInvalidException(
                                $"Không tìm thấy nền tảng giao hàng '{request.ShippingType}' hoặc đã bị tắt.");

                        // 2. Lấy tất cả giá FoodApp của các sản phẩm trong đơn (chỉ lấy giá đang Active)
                        var productIds = request.SalesOrderItem.Select(x => x.ProductId).ToList();

                        var priceDict = await _unitOfWork.Repository<FoodAppPriceEntity>()
                            .GetQueryable()
                            .Where(x => x.FoodAppPlatformId == platform.Id
                                     && productIds.Contains(x.ProductId)
                                     && x.IsActive)
                            .ToDictionaryAsync(x => x.ProductId, x => x.FoodAppPrice);

                        // 3. Duyệt từng món trong đơn → bắt buộc phải có giá trên nền tảng đó
                        decimal totalFoodApp = 0m;

                        foreach (var item in request.SalesOrderItem)
                        {
                            // Nếu món này chưa có giá trên Grab/ShopeeFood → báo lỗi ngay, không cho tạo đơn
                            if (!priceDict.TryGetValue(item.ProductId, out var foodAppPrice))
                            {
                                var prod = await _productsRepository.GetByIdAsync(item.ProductId);
                                throw new ParameterInvalidException(
                                    $"Sản phẩm '{prod?.Name} ({prod?.SKU})' chưa được cấu hình giá trên {platform.Name}");
                            }

                            var lineTotal = foodAppPrice * item.Quantity;
                            totalFoodApp += lineTotal;

                            // Thêm chi tiết đơn (dùng giá FoodApp)
                            salesOrder.SalesOrderItem!.Add(new SalesOrderItemEntity
                            {
                                Id = Guid.NewGuid(),
                                SalesOrderId = salesOrder.Id,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                UnitPrice = foodAppPrice,
                                TotalPrice = lineTotal
                            });
                        }

                        // 4. Gán tổng tiền – KHÔNG giảm giá, KHÔNG dùng điểm → khách trả đúng bằng tổng
                        salesOrder.TotalAmount = totalFoodApp;
                        salesOrder.Discount = 0m;
                        salesOrder.PaidAmount = totalFoodApp;

                        // 5. BỎ QUA hoàn toàn phần logic cũ (khuyến mãi, điểm tích lũy, trừ kho…)
                        //    → nhảy thẳng xuống cuối để lưu đơn
                        goto SkipNormalOrderLogic;
                    }

                    var now = DateTime.Now;
                    decimal totalDiscount = 0m;
                    var appliedPromotions = new List<SalesOrderPromotionEntity>();

                    foreach (var itemReq in request.SalesOrderItem)
                    {
                        var product = await _productsRepository.GetProductForSalesOrderAsync(itemReq.ProductId);
                        decimal originalPrice = product.Price;
                        decimal finalPrice = originalPrice;
                        decimal discountThisItem = 0m;

                        // BƯỚC 1: TẠO ITEM TRƯỚC (luôn tạo, dù có promotion hay không)
                        var salesOrderItem = new SalesOrderItemEntity
                        {
                            Id = Guid.NewGuid(),
                            SalesOrderId = salesOrder.Id,
                            ProductId = itemReq.ProductId,
                            Quantity = itemReq.Quantity,
                            UnitPrice = finalPrice,                    // tạm dùng giá gốc
                            TotalPrice = finalPrice * itemReq.Quantity // tạm tính
                        };
                        salesOrder.SalesOrderItem!.Add(salesOrderItem);

                        // BƯỚC 2: Nếu có promotion → tính lại giá + lưu promotion với SalesOrderItemId đúng
                        if (itemReq.PromotionId.HasValue && itemReq.PromotionId.Value != Guid.Empty)
                        {
                            var promotion = await _unitOfWork.Repository<PromotionEntity>()
                                .GetQueryable()
                                .FirstOrDefaultAsync(p => p.Id == itemReq.PromotionId.Value && p.IsActive && p.StartDate <= now && p.EndDate >= now)
                                ?? throw new ParameterInvalidException("Khuyến mãi không hợp lệ hoặc đã hết hạn");

                            await ValidatePromotionApplicability(promotion, itemReq.ProductId, request.StoreId);

                            decimal discountedPrice = promotion.Type == PromotionType.Percentage
                                ? originalPrice * (1 - promotion.Value / 100m)
                                : originalPrice - promotion.Value;
                            discountedPrice = Math.Max(0, discountedPrice);
                            discountThisItem = originalPrice - discountedPrice;
                            finalPrice = discountedPrice;

                            // CẬP NHẬT LẠI GIÁ CỦA ITEM SAU KHI GIẢM
                            salesOrderItem.UnitPrice = finalPrice;
                            salesOrderItem.TotalPrice = finalPrice * itemReq.Quantity;

                            // LƯU PROMOTION VÀ GÁN ĐÚNG SalesOrderItemId
                            appliedPromotions.Add(new SalesOrderPromotionEntity
                            {
                                Id = Guid.NewGuid(),
                                SalesOrderId = salesOrder.Id,
                                SalesOrderItemId = salesOrderItem.Id,  
                                PromotionId = promotion.Id,
                                DiscountAmount = discountThisItem * itemReq.Quantity
                            });
                        }

                        totalDiscount += discountThisItem * itemReq.Quantity;

                        //salesOrder.SalesOrderItem!.Add(new SalesOrderItemEntity
                        //{
                        //    Id = Guid.NewGuid(),
                        //    SalesOrderId = salesOrder.Id,
                        //    ProductId = itemReq.ProductId,
                        //    Quantity = itemReq.Quantity,
                        //    UnitPrice = finalPrice,
                        //    TotalPrice = finalPrice * itemReq.Quantity
                        //});
                    }

                    // Tổng tiền gốc (trước giảm) → đây mới là TotalAmount đúng nghĩa
                    salesOrder.TotalAmount = request.SalesOrderItem.Sum(item =>
                    {
                        var product = _productsRepository.GetProductForSalesOrderAsync(item.ProductId).Result;
                        return product.Price * item.Quantity;
                    });

                    // Tổng tiền giảm
                    salesOrder.Discount = totalDiscount;

                    // Tiền khách phải trả (sau giảm + điểm) → PaidAmount
                    var amountAfterDiscount = salesOrder.TotalAmount - totalDiscount;
                    salesOrder.PaidAmount = ApplyLoyaltyPoints(
                        request.CustomerId ?? Guid.Empty,
                        amountAfterDiscount,
                        paymentMethod == PaymentMethodType.Online ? OrderStatusType.Pending : OrderStatusType.Confirmed,
                        request.IsUsingPoints,
                        salesOrder.Id);

                    // Thêm đơn + promotion
                    await _unitOfWork.Repository<SalesOrderEntity>().AddAsync(salesOrder);
                    if (appliedPromotions.Any())
                        await _unitOfWork.Repository<SalesOrderPromotionEntity>().AddRangeAsync(appliedPromotions);

                    // CẬP NHẬT USAGE COUNT & LIMIT – CHỈ TÍNH 1 LẦN CHO MỖI PROMOTION (không phụ thuộc số item)
                    var promotionUsageDict = request.SalesOrderItem
                        .Where(x => x.PromotionId.HasValue && x.PromotionId.Value != Guid.Empty)
                        .GroupBy(x => x.PromotionId!.Value)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Sum(x => x.Quantity)
                        );

                    foreach (var kvp in promotionUsageDict)
                    {
                        var promotionId = kvp.Key;
                        var totalAppliedQuantity = kvp.Value;

                        var promotion = await _unitOfWork.Repository<PromotionEntity>()
                            .GetByIdAsync(promotionId);

                        if (promotion == null) continue;

                        promotion.UsageCount += totalAppliedQuantity;

                        //if (promotion.UsageLimit.HasValue && promotion.UsageCount > promotion.UsageLimit.Value)
                        //{
                        //    throw new ParameterInvalidException(
                        //        $"Mã khuyến mãi '{promotion.Code}' đã hết lượt sử dụng. " +
                        //        $"Đã dùng: {promotion.UsageCount - totalAppliedQuantity}, " +
                        //        $"Cần dùng thêm: {totalAppliedQuantity}, " +
                        //        $"Giới hạn: {promotion.UsageLimit.Value}");
                        //}

                        if (promotion.UsageLimit > 0 && promotion.UsageCount + totalAppliedQuantity > promotion.UsageLimit)
                        {
                            throw new ParameterInvalidException(
                                $"Mã khuyến mãi '{promotion.Code}' đã hết lượt sử dụng. " +
                                $"Đã dùng: {promotion.UsageCount}, " +
                                $"Cần dùng thêm: {totalAppliedQuantity}, " +
                                $"Giới hạn: {promotion.UsageLimit}");
                        }

                        _unitOfWork.Repository<PromotionEntity>().UpdateWithoutSave(promotion);
                    }
                    SkipNormalOrderLogic:; //Nhảy xuống đây
                    // Trừ kho + nguyên liệu nếu là COD
                    if (paymentMethod == PaymentMethodType.COD)
                    {
                        var productIds = request.SalesOrderItem.Select(x => x.ProductId).ToList();
                        var inventories = await _unitOfWork.Repository<InventoryEntity>()
                            .GetQueryable()
                            .Where(x => x.StoreId == request.StoreId && productIds.Contains(x.ProductId))
                            .ToListAsync();

                        foreach (var item in request.SalesOrderItem)
                        {
                            var inv = inventories.FirstOrDefault(x => x.ProductId == item.ProductId)
                                ?? throw new NotFoundException($"Không đủ tồn kho cho sản phẩm {item.ProductId}");
                            inv.Quantity -= item.Quantity;
                            _unitOfWork.Repository<InventoryEntity>().UpdateWithoutSave(inv);
                        }

                        await _productIngredientsService.DeductIngredientAsync(
                            request.SalesOrderItem.Select(i => new DeductIngredientModel
                            {
                                StoreId = request.StoreId,
                                ProductId = i.ProductId,
                                Quantity = i.Quantity
                            }).ToList()
                        );
                    }

                    // Tính PaidAmount
                    //salesOrder.PaidAmount = ApplyLoyaltyPoints(
                    //    request.CustomerId ?? Guid.Empty,
                    //    salesOrder.TotalAmount,
                    //    paymentMethod == PaymentMethodType.Online ? OrderStatusType.Pending : OrderStatusType.Confirmed,
                    //    request.IsUsingPoints);
                    // ← Kết thúc transaction
                    // Label để goto khi là đơn FoodApp
                    //SkipNormalOrderLogic:; //Nhảy xuống đây

                    // Thêm đơn hàng vào DB
                    await _unitOfWork.Repository<SalesOrderEntity>().AddAsync(salesOrder);
                });
                // DÒNG CỨU CẢ THẾ GIỚI – BẮT BUỘC CÓ
                await _unitOfWork.SaveChangesAsync();

                // Gán result sau khi đã lưu thành công
                result = paymentMethod == PaymentMethodType.Online
                    ? new { orderId = salesOrder.Id }
                    : new { orderId = salesOrder.Id, isUsingPoints = request.IsUsingPoints };
            });

            // Tạo QR (nếu Online) – chạy sau khi SaveChanges thành công
            if (paymentMethod == PaymentMethodType.Online)
            {
                var bank = (await _unitOfWork.Repository<BankEntity>().GetAllAsync()).FirstOrDefault()!;
                var apiRequest = new ApiRequest
                {
                    acqId = bank.acqId,
                    accountNo = bank.accountNo,
                    accountName = bank.accountName,
                    amount = (int)Math.Round(salesOrder.PaidAmount),
                    format = bank.format,
                    template = bank.template
                };
                var imageBytes = _vietQRPaymentService.GetVietQRImage(apiRequest);
                var base64Image = Convert.ToBase64String(imageBytes);
                result = new { orderId = salesOrder.Id, qr = $"data:image/png;base64,{base64Image}" };
            }

            return OperationResult.SuccessResult("Tạo đơn hàng thành công", result);
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult("Tạo đơn hàng thất bại: " + ex.GetBaseException().Message);
        }
    }

    private async Task ValidatePromotionApplicability(PromotionEntity promotion, Guid productId, Guid storeId)
    {
        // Kiểm tra xem promotion có scope nào không → nếu không có = Global → luôn hợp lệ
        var storeScopes = await _unitOfWork.Repository<PromotionStoreEntity>()
            .GetQueryable()
            .Where(x => x.PromotionId == promotion.Id)
            .ToListAsync();

        var productScopes = await _unitOfWork.Repository<PromotionProductEntity>()
            .GetQueryable()
            .Where(x => x.PromotionId == promotion.Id)
            .ToListAsync();

        var groupScopes = await _unitOfWork.Repository<PromotionProductGroupEntity>()
            .GetQueryable()
            .Where(x => x.PromotionId == promotion.Id)
            .ToListAsync();

        if (storeScopes.Count == 0 && productScopes.Count == 0 && groupScopes.Count == 0)
            return; // Global → OK

        // Có scope → phải match ít nhất 1
        bool matchStore = storeScopes.Any(x => x.StoreId == storeId);
        bool matchProduct = productScopes.Any(x => x.ProductId == productId);

        bool matchGroup = false;
        var product = await _unitOfWork.Repository<ProductEntity>().GetByIdAsync(productId);
        if (product?.ProductGroupId != null)
        {
            matchGroup = groupScopes.Any(x => x.ProductGroupId == product.ProductGroupId);
        }

        if (!matchStore && !matchProduct && !matchGroup)
            throw new ParameterInvalidException("Khuyến mãi không áp dụng được cho sản phẩm hoặc cửa hàng này");
    }

    public async Task<OperationResult> UpdateOrderStatus(UpdateOrderStatusModel model)
    {
        try
        {
            var deductIngredient = new List<DeductIngredientModel>();
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                var salesOrderEntity = await _unitOfWork.Repository<SalesOrderEntity>().GetByIdAsync(model.OrderId);

                var newStatus = Enum.Parse<OrderStatusType>(model.OrderStatus!, true);

                if (newStatus == OrderStatusType.Confirmed)
                {
                    var order = await _repository.GetSalesOrderByIdForUpdateSOAsync(model.OrderId);

                    var productIds = order.SalesOrderItem.Select(i => i.ProductId).Distinct().ToList();

                    var inventories = await _unitOfWork.Repository<InventoryEntity>().GetQueryable()
                        .Where(x => x.StoreId == model.StoreId && productIds.Contains(x.ProductId))
                        .ToListAsync();

                    foreach (var item in order.SalesOrderItem)
                    {
                        var inventory = inventories.FirstOrDefault(x => x.ProductId == item.ProductId);

                        if (inventory!.Quantity < item.Quantity)
                            throw new ParameterInvalidException($"Not enough inventory for product {item.ProductId}");

                        inventory.Quantity -= item.Quantity;

                        deductIngredient.Add(new DeductIngredientModel
                        {
                            StoreId = model.StoreId,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        });
                    }

                    foreach (var inventory in inventories)
                    {
                        _unitOfWork.Repository<InventoryEntity>().UpdateWithoutSave(inventory);
                    }
                }

                salesOrderEntity.OrderStatus = newStatus;
                salesOrderEntity.PaidAmount = newStatus == OrderStatusType.Confirmed
                    ? ApplyLoyaltyPoints(salesOrderEntity.CustomerId ?? Guid.Empty, salesOrderEntity.TotalAmount, newStatus, model.IsUsingPoints, salesOrderEntity.Id)
                    : salesOrderEntity.PaidAmount;
                _unitOfWork.Repository<SalesOrderEntity>().UpdateWithoutSave(salesOrderEntity);
                await _productIngredientsService.DeductIngredientAsync(deductIngredient);
            });

            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(SalesOrder), model.StoreId));
        }
        catch (Exception ex)
        {
            return OperationResult.FailureResult(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(SalesOrder)) + ": " + ex.Message);
        }
    }
    private decimal ApplyLoyaltyPoints(Guid customerId, decimal totalBeforeLoyaltyPoint, OrderStatusType orderStatus, bool isUsingPoints, Guid salesOrderId)
    {
        var loyaltyPointRepo = _unitOfWork.Repository<LoyaltyPointEntity>();
        var loyaltyPoint = loyaltyPointRepo.GetQueryable()
            .FirstOrDefault(x => x.CustomerId == customerId);

        if (loyaltyPoint == null || customerId == Guid.Empty)
            return totalBeforeLoyaltyPoint;

        var config = GetActiveLoyaltyConfigAsync().Result;
        if (config == null)
            return totalBeforeLoyaltyPoint;  // Không có config → giữ nguyên, không áp điểm

        var earnRate = config.EarnRate;
        var redeemRate = config.RedeemRate;

        if (orderStatus == OrderStatusType.Pending)
        {
            return CalculateDiscountedTotal(totalBeforeLoyaltyPoint, loyaltyPoint.Points, isUsingPoints, redeemRate);
            //return CalculateDiscountedTotal(totalBeforeLoyaltyPoint, loyaltyPoint.Points, isUsingPoints);
        }

        if (orderStatus == OrderStatusType.Confirmed)
        {
            return ProcessFinalLoyaltyPoints(loyaltyPoint, totalBeforeLoyaltyPoint, isUsingPoints, earnRate, redeemRate, salesOrderId);
            //return ProcessFinalLoyaltyPoints(loyaltyPoint, totalBeforeLoyaltyPoint, isUsingPoints);
        }

        return totalBeforeLoyaltyPoint;
    }
    private decimal CalculateDiscountedTotal(decimal total, int availablePoints, bool isUsingPoints, decimal redeemRate)
    {
        if (!isUsingPoints || availablePoints <= 0)
            return total;

        //decimal discount = Math.Min(availablePoints, total);
        var discount = Math.Min(availablePoints * redeemRate, total);
        return total - discount;
    }
    private decimal ProcessFinalLoyaltyPoints(LoyaltyPointEntity loyaltyPoint, decimal total, bool isUsingPoints, decimal earnRate, decimal redeemRate, Guid salesOrderId)
    {
        decimal discountedTotal = total;
        int pointsUsed = 0; // ← THÊM DÒNG NÀY

        if (isUsingPoints && loyaltyPoint.Points > 0)
        {
            var maxDiscountByPoints = loyaltyPoint.Points * redeemRate;
            var discount = Math.Min(maxDiscountByPoints, total);
            discountedTotal -= discount;
            //loyaltyPoint.Points -= (int)discount;
            pointsUsed = (int)Math.Floor(discount / redeemRate);
            loyaltyPoint.Points -= pointsUsed; ;  // Trừ điểm dùng
        }

        int earnedPoints = (int)Math.Floor(discountedTotal / earnRate);  // Cộng điểm mới
        //int earnedPoints = (int)Math.Floor(discountedTotal / 10);
        loyaltyPoint.Points += earnedPoints;

        _unitOfWork.Repository<LoyaltyPointEntity>().UpdateWithoutSave(loyaltyPoint);

        if (loyaltyPoint.CustomerId != Guid.Empty)
        {
            // Fire and forget – không await, không làm chậm đơn
            _ = AddLoyaltyTransactionAsync(loyaltyPoint.CustomerId, salesOrderId, pointsUsed, earnedPoints);
        }

        return discountedTotal;
    }

    private async Task<LoyaltyPointConfigEntity?> GetActiveLoyaltyConfigAsync()
    {
        return await _unitOfWork.Repository<LoyaltyPointConfigEntity>()
            .GetQueryable()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreateDate)  // Mới nhất
            .FirstOrDefaultAsync();
    }

    private async Task<SalesOrderEntity> ApplyPromotion(List<CreateSalesOrderPromotionModel>? salesOrderPromotions, SalesOrderEntity salesOrder)
    {
        if (salesOrderPromotions != null && salesOrderPromotions.Count > 0)
        {
            var promotionIds = salesOrderPromotions.Select(x => x.PromotionId).ToList();

            var promotions = await _unitOfWork
                .Repository<PromotionEntity>()
                .GetQueryable()
                .Where(p => promotionIds.Contains(p.Id))
                .ToListAsync();

            // Map nhanh để tránh foreach + FirstOrDefault tốn công
            var promotionDict = promotions.ToDictionary(p => p.Id, p => p);

            foreach (var promotionModel in salesOrderPromotions)
            {
                if (!promotionDict.TryGetValue(promotionModel.PromotionId, out var promotion))
                {
                    throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(PromotionEntity), promotionModel.PromotionId));
                }

                decimal discountAmount = CalculateTotalAmountByPromotionType(promotion.Type, salesOrder.TotalAmount, promotion.Value);

                salesOrder.Discount += discountAmount;
                salesOrder.TotalAmount -= discountAmount;
                var salesOrderPromotion = new SalesOrderPromotionEntity
                {
                    Id = Guid.NewGuid(),
                    SalesOrderId = salesOrder.Id,
                    PromotionId = promotion.Id,
                    DiscountAmount = discountAmount
                };
                await _unitOfWork.Repository<SalesOrderPromotionEntity>().AddAsync(salesOrderPromotion);
            }
        }
        else
        {
            salesOrder.Discount = 0;
            salesOrder.TotalAmount = salesOrder.TotalAmount;
        }
        return salesOrder;
    }
    private decimal CalculateTotalAmountByPromotionType(PromotionType type, decimal totalBeforeDiscount, decimal promotionValue)
    {
        return type switch
        {
            PromotionType.Percentage => totalBeforeDiscount * (promotionValue / 100),
            PromotionType.FixedAmount => totalBeforeDiscount - promotionValue,
            _ => 0,
        };
    }
    // Application/SalesOrdersService.cs

    public async Task<object> GetOrderCountByStoreAsync(Guid storeId, ShippingType? platform = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _unitOfWork.Repository<SalesOrderEntity>()
            .GetQueryable()
            .Where(x => x.StoreId == storeId);

        if (platform.HasValue && platform != ShippingType.None)
            query = query.Where(x => x.ShippingType == platform.Value);
        if (fromDate.HasValue) query = query.Where(x => x.CreateDate >= fromDate.Value.Date);
        if (toDate.HasValue) query = query.Where(x => x.CreateDate < toDate.Value.AddDays(1).Date);

        var count = await query.CountAsync();
        return new { TotalOrders = count };
    }

    public async Task<Dictionary<string, object>> GetOrderCountByPlatformAsync(Guid storeId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _unitOfWork.Repository<SalesOrderEntity>()
            .GetQueryable()
            .Where(x => x.StoreId == storeId && x.ShippingType != ShippingType.None);

        if (fromDate.HasValue) query = query.Where(x => x.CreateDate >= fromDate.Value.Date);
        if (toDate.HasValue) query = query.Where(x => x.CreateDate < toDate.Value.AddDays(1).Date);

        var result = await query
            .GroupBy(x => x.ShippingType)
            .Select(g => new
            {
                Platform = g.Key.ToString(),
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.Platform, x => (object)new { TotalOrders = x.Count });

        return result;
    }

    public async Task<Dictionary<string, object>> GetOrderCountByDateAsync(Guid storeId, ShippingType? platform = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _unitOfWork.Repository<SalesOrderEntity>()
            .GetQueryable()
            .Where(x => x.StoreId == storeId);

        if (platform.HasValue && platform != ShippingType.None)
            query = query.Where(x => x.ShippingType == platform.Value);
        if (fromDate.HasValue) query = query.Where(x => x.CreateDate >= fromDate.Value.Date);
        if (toDate.HasValue) query = query.Where(x => x.CreateDate < toDate.Value.AddDays(1).Date);

        var result = await query
            .GroupBy(x => x.CreateDate.Date)
            .Select(g => new
            {
                Date = g.Key.ToString("dd/MM/yyyy"),
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToDictionaryAsync(x => x.Date, x => (object)new { TotalOrders = x.Count });

        return result;
    }

    public async Task<object> GetRevenueByStoreAsync(Guid storeId, ShippingType? platform = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _unitOfWork.Repository<SalesOrderEntity>()
            .GetQueryable()
            .Where(x => x.StoreId == storeId);

        if (platform.HasValue && platform != ShippingType.None)
            query = query.Where(x => x.ShippingType == platform.Value);
        if (fromDate.HasValue) query = query.Where(x => x.CreateDate >= fromDate.Value.Date);
        if (toDate.HasValue) query = query.Where(x => x.CreateDate < toDate.Value.AddDays(1).Date);

        var revenue = await query.SumAsync(x => x.PaidAmount);
        return new { TotalRevenue = revenue };
    }

    public async Task<Dictionary<string, object>> GetRevenueByPlatformAsync(Guid storeId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _unitOfWork.Repository<SalesOrderEntity>()
            .GetQueryable()
            .Where(x => x.StoreId == storeId && x.ShippingType != ShippingType.None);

        if (fromDate.HasValue) query = query.Where(x => x.CreateDate >= fromDate.Value.Date);
        if (toDate.HasValue) query = query.Where(x => x.CreateDate < toDate.Value.AddDays(1).Date);

        var result = await query
            .GroupBy(x => x.ShippingType)
            .Select(g => new
            {
                Platform = g.Key.ToString(),
                Revenue = g.Sum(x => x.PaidAmount)
            })
            .ToDictionaryAsync(x => x.Platform, x => (object)new { TotalRevenue = x.Revenue });

        return result;
    }

    public async Task<Dictionary<string, object>> GetRevenueByDateAsync(Guid storeId, ShippingType? platform = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _unitOfWork.Repository<SalesOrderEntity>()
            .GetQueryable()
            .Where(x => x.StoreId == storeId);

        if (platform.HasValue && platform != ShippingType.None)
            query = query.Where(x => x.ShippingType == platform.Value);
        if (fromDate.HasValue) query = query.Where(x => x.CreateDate >= fromDate.Value.Date);
        if (toDate.HasValue) query = query.Where(x => x.CreateDate < toDate.Value.AddDays(1).Date);

        var result = await query
            .GroupBy(x => x.CreateDate.Date)
            .Select(g => new
            {
                Date = g.Key.ToString("dd/MM/yyyy"),
                Revenue = g.Sum(x => x.PaidAmount)
            })
            .OrderBy(x => x.Date)
            .ToDictionaryAsync(x => x.Date, x => (object)new { TotalRevenue = x.Revenue });

        return result;
    }

    private async Task AddLoyaltyTransactionAsync(Guid customerId, Guid salesOrderId, int pointsUsed, int pointsEarned)
    {
        if (pointsUsed == 0 && pointsEarned == 0) return; // Không có gì thì thôi

        var transaction = new LoyaltyTransactionEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            SalesOrderId = salesOrderId,
            PointsUsed = pointsUsed,
            PointsEarned = pointsEarned,
            CreateDate = DateTime.UtcNow
        };

        await _unitOfWork.Repository<LoyaltyTransactionEntity>().AddAsync(transaction);
    }
}