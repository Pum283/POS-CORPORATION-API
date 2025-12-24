namespace Domain;

[Table("Users")]
public class UserEntity : EntityBase<Guid>
{
    public Guid? StoreId { get; set; }
    public StoreEntity? Store { get; set; }

    [MaxLength(AppConstants.MaxLengthName)]
    [Required]
    public string Name { get; set; } = string.Empty;
    [MaxLength(AppConstants.MaxLengthEmail)]
    [Required]
    public string Email { get; set; } = string.Empty;
    [MaxLength(AppConstants.MaxLengthUrl)]
    public string? Picture { get; set; }
    [Required]
    public string Password { get; set; } = string.Empty;
    //public UserStatusType Status { get; set; }
    public string? Gender { get; set; }
    public DateTime? DeleteTime { get; set; }
    public bool IsDeleted { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? Phone { get; set; }
    public DateTime DOB { get; set; }
    public string? Address { get; set; }
    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
    public ICollection<AuditLogEntity>? AuditLogs { get; set; }

    public static UserEntity Create(Guid id, string name, string email, string password, bool IsDeleted)
    {
        var userEntity = new UserEntity
        {
            Id = id,
            Name = name,
            Email = email,
            Password = password,
            IsDeleted = IsDeleted
        };
        return userEntity;
    }
}