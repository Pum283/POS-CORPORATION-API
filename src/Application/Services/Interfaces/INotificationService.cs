namespace Application;

public interface INotificationService
{
    Task SendAndSaveAsync(Guid storeId, string title, string message, Guid? documentId = null, NotificationType notificationType = NotificationType.General);
    Task<NotificationQueryResultModel> GetByStoreAsync(Guid storeId, QueryInfo queryInfo);
    Task<OperationResult> MarkAsReadAsync(Guid notificationId);
}
