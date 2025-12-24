namespace Infrastructure;

public interface INotificationRepository : IRepositoryBase<NotificationEntity>
{
    Task<NotificationQueryResultModel> GetByStoreAsync(Guid storeId, QueryInfo queryInfo);
}
