namespace API.Validators;

public class QueryInfoValidator : AbstractValidator<QueryInfo>
{
    public QueryInfoValidator()
    {
        RuleFor(x => x.Top).InclusiveBetween(1, 1000);
        RuleFor(x => x.Skip).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SearchText).MaximumLength(100);
        RuleFor(x => x.OrderBy).MaximumLength(50);
    }
}
