namespace Domain;

public class NotificationQueryResultModel
{
    public int UnreadCount { get; set; }
    public QueryResult<NotificationModel> Data { get; set; } = new();
}

