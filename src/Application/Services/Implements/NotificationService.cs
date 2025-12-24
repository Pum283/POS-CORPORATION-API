namespace Application;

[RegisterService(typeof(INotificationService))]
public class NotificationService(
    INotificationRepository _repository,
    IUnitOfWork _unitOfWork,
    IHubContext<NotificationHub> _hubContext) : INotificationService
{
    public async Task<NotificationQueryResultModel> GetByStoreAsync(Guid storeId, QueryInfo queryInfo)
      => await _repository.GetByStoreAsync(storeId, queryInfo);
    public async Task SendAndSaveAsync(Guid storeId, string title, string message, Guid? documentId = null, NotificationType notificationType = NotificationType.General)
    {
        var entity = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            StoreId = storeId,
            DocumentId = documentId,
            NotificationType = notificationType,
            Title = title,
            Message = message,
            IsRead = false
        };

        await _unitOfWork.Repository<NotificationEntity>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        await _hubContext.Clients.Group(storeId.ToString()).SendAsync("ReceiveNotification", new
        {
            entity.Id,
            entity.Title,
            entity.Message,
            entity.Image,
            entity.CreateDate
        });
    }

    public async Task<OperationResult> MarkAsReadAsync(Guid notificationId)
    {
        var entity = await _unitOfWork.Repository<NotificationEntity>().GetByIdAsync(notificationId);

        entity.IsRead = true;
        await _unitOfWork.Repository<NotificationEntity>().UpdateAsync(entity);
        if(!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.FailureResult("Mark notification as read failed");
        }
        return OperationResult.SuccessResult();
    }
}
