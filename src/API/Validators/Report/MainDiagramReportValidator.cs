namespace API;

public class MainDiagramReportValidator : AbstractValidator<MainDiagramReportQuery>
{
    public MainDiagramReportValidator()
    {
        RuleFor(x => x.Date)
            .GreaterThan(DateTime.MinValue)
            .WithMessage(ErrorMessageBase.GreaterThan);

        RuleFor(x => x.ShippingType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(ShippingType)).Contains(value))
            .WithMessage(ErrorMessageBase.InvalidFormat);
    }
}
