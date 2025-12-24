namespace API;

public class CreateLoyaltyPointValidator : AbstractValidator<CreateLoyaltyPointModel>
{
    public CreateLoyaltyPointValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotNull().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Points)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
