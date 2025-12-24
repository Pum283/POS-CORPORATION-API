namespace Domain;

[Table("AuditLogs")]
public class AuditLogEntity : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
    public string? Action { get; set; } // Hành động: tạo, sửa, xóa...
    public string? EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string? Description { get; set; }
}
