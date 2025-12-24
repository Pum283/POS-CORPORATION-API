namespace Domain;

[Table("Notifications")]
public class NotificationEntity : EntityBase<Guid>
{
    public NotificationType NotificationType { get; set; }
    public Guid? StoreId { get; set; }
    public  Guid? UserId { get; set; }
    public  Guid? DocumentId { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? Image { get; set; }
    public bool IsRead { get; set; }
}
