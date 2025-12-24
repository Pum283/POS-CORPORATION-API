namespace API;

public class CreatePromotionModelValidator : AbstractValidator<CreatePromotionModel>
{
    public CreatePromotionModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(255).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.PromotionType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(PromotionType)).Contains(value));

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must((m, end) => end > m.StartDate)
            .WithMessage("EndDate must be greater than StartDate.");

        RuleFor(x => x.UsageLimit)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThanOrEqual);

        RuleFor(x => x.ProductScope)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(ProductTargetScopeType)).Contains(value));

        RuleFor(x => x.StoreScope)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(StoreTargetScopeType)).Contains(value));
    }
}
