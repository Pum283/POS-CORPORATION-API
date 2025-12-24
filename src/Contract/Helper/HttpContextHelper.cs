namespace Contract;

public static class HttpContextHelper
{
    public static Guid GetCurrentUserId(this HttpContext httpContext)
    {
        var currentId = httpContext?.User.FindFirstValue("Id");
        Guid.TryParse(currentId, out var userId);
        return userId;
    }
    public static Guid GetCurrentStoreId(this HttpContext httpContext)
    {
        var currentStoreId = httpContext?.User.FindFirstValue("StoreId");
        Guid.TryParse(currentStoreId, out var storeId);
        return storeId;
    }
    public static string GetCurrentUserName(this HttpContext httpContext)
    {
        var currentUserName = httpContext?.User.FindFirstValue(ClaimTypes.Name);
        return currentUserName ?? "";
    }
    public static string GetCurrentUserEmail(this HttpContext httpContext)
    {
        var currentUserEmail = httpContext?.User.FindFirstValue(ClaimTypes.Email);
        return currentUserEmail ?? "";
    }
    public static string GetCurrentPicture(this HttpContext httpContext)
    {
        var currentUserPicture = httpContext?.User.FindFirstValue("Picture");
        return currentUserPicture ?? "";
    }
    public static string GetCurrentUserRole(this HttpContext httpContext)
    {
        var currentUserRole = httpContext?.User.FindFirstValue(ClaimTypes.Role);
        return currentUserRole ?? "";
    }
}
