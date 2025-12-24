namespace Infrastructure;

[RegisterService(typeof(INotificationRepository))]
public class NotificationRepository(EfApplicationDbContext context) : RepositoryBase<NotificationEntity>(context), INotificationRepository
{
    private readonly EfApplicationDbContext _context = context;

    public async Task<NotificationQueryResultModel> GetByStoreAsync(Guid storeId, QueryInfo queryInfo)
    {
        var query = _context.Notifications.AsNoTracking()
            .Where(x => x.StoreId == storeId)
            .AsQueryable();

        var entities = await query
            .OrderByDescending(x => x.CreateDate)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .Select(x => new NotificationModel
            {
                Id = x.Id,
                StoreId = x.StoreId,
                UserId = x.UserId,
                DocumentId = x.DocumentId,
                NotificationType = x.NotificationType.ToString(),
                Title = x.Title,
                Message = x.Message,
                IsRead = x.IsRead,
                CreateDate = x.CreateDate
            })
            .ToListAsync();

        var totalCount = queryInfo.NeedTotalCount ? await query.CountAsync() : 0;

        var unreadCount = await query.CountAsync(x => !x.IsRead);

        var data = new QueryResult<NotificationModel>
        {
            Data = entities,
            TotalCount = totalCount
        };
        return new NotificationQueryResultModel
        {
            Data = data,
            UnreadCount = unreadCount
        };
    }

}
