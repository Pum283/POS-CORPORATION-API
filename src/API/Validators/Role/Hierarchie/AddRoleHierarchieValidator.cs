namespace API;

public class AddRoleHierarchieValidator : AbstractValidator<AddOrUpdateRoleHierarchieModel>
{
    public AddRoleHierarchieValidator()
    {
        RuleFor(x => x.ParentRoleId)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.ChildRoleId)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required)
            .Must((model, childRoleId) => childRoleId != model.ParentRoleId)
            .WithMessage("Child role ID cannot be the same as Parent role ID.");
    }
}
