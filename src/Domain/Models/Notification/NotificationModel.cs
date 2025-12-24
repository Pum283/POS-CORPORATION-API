namespace Domain;

public class NotificationModel
{
    public Guid Id { get; set; }
    public Guid? StoreId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? DocumentId { get; set; }
    public string NotificationType { get; set; } = null!;
    public string? Title { get; set; }
    public string? Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreateDate { get; set; }
}
