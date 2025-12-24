namespace API;

public class DocumentReportByTransTypeValidator : AbstractValidator<DocumentReportByTransTypeQuery>
{
    public DocumentReportByTransTypeValidator()
    {
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .WithMessage(ErrorMessageBase.LessThan);

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage(ErrorMessageBase.GreaterThan);

        RuleFor(x => x.TransType)
            .NotEmpty().WithMessage(ErrorMessageBase.Empty)
            .Must(value => Enum.GetNames(typeof(TransTypeStaff)).Contains(value))
            .WithMessage(ErrorMessageBase.InvalidFormat);
    }
}
