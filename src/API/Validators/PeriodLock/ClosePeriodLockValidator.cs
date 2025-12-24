namespace API;

public class ClosePeriodLockValidator : AbstractValidator<ClosePeriodLockModel>
{
    public ClosePeriodLockValidator()
    {
        RuleFor(x => x.ClosingBalance)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);
    }
}
