using System.Security.Claims;

namespace API;

public class PermissionsMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var _permissionService = context.RequestServices.GetRequiredService<IPermissionService>();
        var _storeAccountantService = context.RequestServices.GetRequiredService<IStoreAccountantsService>();

        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        var permissionAttributes = endpoint.Metadata.GetOrderedMetadata<RequirePermissionsAttribute>();

        if (permissionAttributes == null || !permissionAttributes.Any())
        {
            await _next(context);
            return;
        }

        var requiredPermissions = permissionAttributes
            .SelectMany(attr => attr.RequiredPermissions)
            .Distinct()
            .ToList();

        var user = context.User;

        var userIdStr = user.FindFirst("Id")?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            throw new UnauthenticatedException("Invalid user token.");
        }

        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole == "Accountant")
        {
            Guid? storeId = null;
            if (context.Request.Headers.TryGetValue("X-Store-Id", out var storeIdHeader))
            {
                if (!Guid.TryParse(storeIdHeader.ToString(), out var parsedStoreId))
                {
                    throw new BadRequestException("StoreId trong header không hợp lệ.");
                }

                storeId = parsedStoreId;
            }
            if (storeId.HasValue)
            {
                var hasAccess = await _storeAccountantService.HasStoreAccessAsync(userId, storeId.Value);
                if (!hasAccess)
                {
                    throw new ForbiddenException("Bạn không có quyền truy cập cửa hàng này.");
                }
            }
        }

        var userPermissions = await _permissionService.GetPermissionsForUserAsync(userId);

        var hasAllPermissions = requiredPermissions.All(p => userPermissions.Contains(p));

        if (!hasAllPermissions)
        {
            throw new ForbiddenException(ErrorMessageBase.Forbidden);
        }

        await _next(context);
    }
}
