namespace API;

public class GetRevenueReportValidator : AbstractValidator<GetRevenueReportQuery>
{
    public GetRevenueReportValidator()
    {
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .WithMessage(ErrorMessageBase.LessThan);

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage(ErrorMessageBase.GreaterThan);

        RuleFor(x => x)
            .Must(ZeroOrOnlyOneSelected)
            .WithMessage("Only choice 1 in 2: ProductId or ProductGroupId.");
    }
    private bool ZeroOrOnlyOneSelected(GetRevenueReportQuery query)
    {
        bool hasProductId = query.ProductId.HasValue && query.ProductId.Value != Guid.Empty;
        bool hasProductGroupId = query.ProductGroupId.HasValue && query.ProductGroupId.Value != Guid.Empty;

        return !(hasProductId && hasProductGroupId);
    }
}
