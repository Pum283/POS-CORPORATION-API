namespace API;

public class GetPromotionsQueryValidator : AbstractValidator<GetPromotionsQuery>
{
    public GetPromotionsQueryValidator()
    {
        RuleFor(x => x.PromotionType)
            .Must(value => Enum.GetNames(typeof(PromotionType)).Contains(value)).When(x => x.PromotionType != null);
        RuleFor(x => x.EndDate)
            .NotNull()
            .WithMessage("EndDate is required when StartDate is provided.")
            .When(x => x.StartDate.HasValue);

        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.StartDate <= x.EndDate)
            .WithMessage("StartDate must be less than or equal to EndDate.");
    }
}
