namespace API;

public class CreateOrUpdateIngredientValidator : AbstractValidator<CreateOrUpdateIngredientModel>
{
    public CreateOrUpdateIngredientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(UnitType)).Contains(value)).WithMessage(ErrorMessageBase.InvalidFormat);
    }
}
