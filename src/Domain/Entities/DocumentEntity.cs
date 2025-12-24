namespace Domain;

[Table("Documents")]
public class DocumentEntity : EntityBase<Guid>
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    [MaxLength(AppConstants.MaxLengthName)]
    public required string DocumentName { get; set; }
    public DocumentStatusType Status { get; set; }
    [Required]
    public required string Description { get; set; }
}
