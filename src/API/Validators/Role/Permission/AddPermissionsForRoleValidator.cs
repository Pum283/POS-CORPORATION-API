namespace API;

public class AddPermissionsForRoleValidator : AbstractValidator<AddPermissionsForRoleModel>
{
    public AddPermissionsForRoleValidator()
    {
        RuleFor(model => model.RoleId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(roleId => roleId != Guid.Empty).WithMessage(ErrorMessageBase.Empty);
        RuleFor(model => model.PermissionId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(permissionIds => permissionIds.Count > 0).WithMessage(ErrorMessageBase.Empty)
            .ForEach(permissionId =>
                permissionId.NotEmpty().WithMessage(ErrorMessageBase.Empty)
                            .Must(id => id != Guid.Empty).WithMessage(ErrorMessageBase.Empty));
    }
}
