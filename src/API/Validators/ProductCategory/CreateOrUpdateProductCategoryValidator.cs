namespace API;

public class CreateOrUpdateProductCategoryValidator : AbstractValidator<CreateOrUpdateProductCategoryModel>
{
    public CreateOrUpdateProductCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);
    }
}
