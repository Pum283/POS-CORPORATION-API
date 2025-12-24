namespace API;

public class CreateOrUpdateProductGroupValidator : AbstractValidator<CreateOrUpdateProductGroupModel>
{
    public CreateOrUpdateProductGroupValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);
        RuleFor(x => x.Description)
            .MaximumLength(100).WithMessage(ErrorMessageBase.MaxLength);
        RuleFor(x => x.ProductCategoryId)
            .NotNull().WithMessage(ErrorMessageBase.Required);
    }
}
