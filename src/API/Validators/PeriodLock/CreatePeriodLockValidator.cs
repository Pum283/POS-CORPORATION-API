namespace API;

public class CreatePeriodLockValidator : AbstractValidator<CreatePeriodLockModel>
{
    public CreatePeriodLockValidator()
    {
        RuleFor(x => x.Shift)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(ShiftType)).Contains(value));
        RuleFor(x => x.OpeningBalance)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);
    }
}
