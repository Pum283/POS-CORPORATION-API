namespace API.Controllers;

[Route("enums")]
public class EnumsController : BaseApiController
{
    [HttpGet("role-types")]
    public IActionResult GetRoleTypes()
    {
        var roleTypes = EnumHelper.GetEnumList<RoleType>();
        return Ok(roleTypes);
    }
    [HttpGet("document-statues-types")]
    public IActionResult GetDocumentStatusTypes()
    {
        var productCategories = EnumHelper.GetEnumList<DocumentStatusType>();
        return Ok(productCategories);
    }
    [HttpGet("order-statuses-types")]
    public IActionResult GetOrderStatusesTypes()
    {
        var orderStatuses = EnumHelper.GetEnumList<OrderStatusType>();
        return Ok(orderStatuses);
    }
    [HttpGet("payment-methods-types")]
    public IActionResult GetPaymentMethodsTypes()
    {
        var paymentMethods = EnumHelper.GetEnumList<PaymentMethodType>();
        return Ok(paymentMethods);
    }
    [HttpGet("permission-types")]
    public IActionResult GetPermissionTypes()
    {
        var permissionTypes = EnumHelper.GetEnumList<PermissionType>();
        return Ok(permissionTypes);
    }
    [HttpGet("promotion-types")]
    public IActionResult GetPromotionTypes()
    {
        var promotionTypes = EnumHelper.GetEnumList<PromotionType>();
        return Ok(promotionTypes);
    }
    [HttpGet("trans-types")]
    public IActionResult GetTransTypes()
    {
        var transTypes = EnumHelper.GetEnumList<TransType>();
        return Ok(transTypes);
    }
    [HttpGet("trans-types-staff")]
    public IActionResult GetTransTypesStaff()
    {
        var transTypes = EnumHelper.GetEnumList<TransTypeStaff>();
        return Ok(transTypes);
    }
    [HttpGet("trans-types-accountant")]
    public IActionResult GetTransTypesAccountant()
    {
        var transTypes = EnumHelper.GetEnumList<TransTypeAccountant>();
        return Ok(transTypes);
    }
    [HttpGet("unit-types")]
    public IActionResult GetUnitTypes()
    {
        var unitTypes = EnumHelper.GetEnumList<UnitType>();
        return Ok(unitTypes);
    }
    [HttpGet("user-status-types")]
    public IActionResult GetUserStatusTypes()
    {
        var userStatusTypes = EnumHelper.GetEnumList<UserStatusType>();
        return Ok(userStatusTypes);
    }
    [HttpGet("shift-types")]
    public IActionResult GetShiftTypesTypes()
    {
        var userShiftTypes = EnumHelper.GetEnumList<ShiftType>();
        return Ok(userShiftTypes);
    }
    [HttpGet("shipping-types")]
    public IActionResult GetShippingTypes()
    {
        var userShiftTypes = EnumHelper.GetEnumList<ShippingType>();
        return Ok(userShiftTypes);
    }
    [HttpGet("product-target-scope-types")]
    public IActionResult GetProductTargetScopes()
    {
        var userShiftTypes = EnumHelper.GetEnumList<ProductTargetScopeType>();
        return Ok(userShiftTypes);
    }
    [HttpGet("store-target-score-types")]
    public IActionResult GetStoreTargetScope()
    {
        var userShiftTypes = EnumHelper.GetEnumList<StoreTargetScopeType>();
        return Ok(userShiftTypes);
    }
    [HttpGet("document-status-types")]
    public IActionResult GetDocumentStatusScope()
    {
        var userShiftTypes = EnumHelper.GetEnumList<DocumentStatusType>();
        return Ok(userShiftTypes);
    }
}
