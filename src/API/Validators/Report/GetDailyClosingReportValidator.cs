namespace API;

public class GetDailyClosingReportValidator : AbstractValidator<GetDailyClosingReportQuery>
{
    public GetDailyClosingReportValidator()
    {
        RuleFor(x => x.Date).GreaterThan(DateTime.MinValue)
            .WithMessage("The date must be greater than the minimum value of DateTime.");
    }
}
