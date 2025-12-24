namespace API.Controllers;

[Route("notifications")]
//[Authorize]
public class NotificationsController(INotificationService _service) : BaseApiController
{
    [HttpGet("{storeId}/store")]
    public async Task<NotificationQueryResultModel> GetNotificationsByStore([FromRoute] Guid storeId, [FromQuery] QueryInfo queryInfo)
    {
        return await _service.GetByStoreAsync(storeId, queryInfo);
    }

    [HttpPatch("{notificationId}/read")]
    public async Task<OperationResult> MarkAsRead(Guid notificationId)
    {
        return await _service.MarkAsReadAsync(notificationId);
    }
}
