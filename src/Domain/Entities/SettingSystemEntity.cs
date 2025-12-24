namespace Domain;

[Table("SettingSystems")]
public class SettingSystemEntity : EntityBase<Guid>
{
    [MaxLength(50)]
    public string? ColorSystem1 { get; set; }

    [MaxLength(50)]
    public string? ColorSystem2 { get; set; }
}
