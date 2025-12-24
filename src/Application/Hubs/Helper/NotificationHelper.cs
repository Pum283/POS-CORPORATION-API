namespace Application;

public class NotificationHelper
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly INotificationService _notificationService;

    public NotificationHelper(IHubContext<NotificationHub> hubContext, INotificationService notificationService)
    {
        _hubContext = hubContext;
        _notificationService = notificationService;
    }

    public async Task SendAndSaveAsync(Guid storeId, string title, string message, Guid? documentId = null, NotificationType notificationType = NotificationType.General)
    {
        // Gửi realtime
        await _hubContext.Clients.Group(storeId.ToString()).SendAsync(
            "ReceiveNotification",
            new
            {
                Title = title,
                Message = message,
                DocumentId = documentId
            }
        );

        // Lưu vào DB
        await _notificationService.SendAndSaveAsync(storeId, title, message, documentId, notificationType);
    }
}
