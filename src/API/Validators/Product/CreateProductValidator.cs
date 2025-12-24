namespace API;

public class CreateProductValidator : AbstractValidator<CreateProductModel>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Length(3, 256).WithMessage(ErrorMessageBase.Range);

        //RuleForEach(x => x.Picture!)
        //   .SetValidator(new IFormFileValidator());
        //RuleFor(x => x.Picture)
        //    .NotEmpty().WithMessage(ErrorMessageBase.Required)
        //    .SetValidator(new IFormFileValidator()!);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MinimumLength(3).WithMessage(ErrorMessageBase.Range);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);

        RuleFor(x => x.Cost)
            .GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);

        RuleFor(x => x.ProductGroupId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
