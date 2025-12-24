namespace API;

public class CreateUserValidator : AbstractValidator<CreateUserModel>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Length(3, 256).WithMessage(ErrorMessageBase.Range);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .EmailAddress().WithMessage(ErrorMessageBase.InvalidEmail)
            .Matches(@"^\S+$").WithMessage(ErrorMessageBase.NotContainSpaces);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Length(6, 18).WithMessage(ErrorMessageBase.Range)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9])\S+$")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, one special character, and no spaces.");

        RuleFor(x => x.RepeatPassword)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Equal(x => x.Password).WithMessage(ErrorMessageBase.MustMatch)
            .Matches(@"^\S+$").WithMessage(ErrorMessageBase.NotContainSpaces);

        RuleFor(x => x.StoreId)
            .NotNull()
            .WithMessage("Store is required for staff.")
            .When(x => x.Role == RoleType.Staff.ToString());

        RuleFor(x => x.StoreId)
            .Must(id => id == null)
            .WithMessage("Store must be null unless role is staff.")
            .When(x => x.Role != RoleType.Staff.ToString());
    }
}
