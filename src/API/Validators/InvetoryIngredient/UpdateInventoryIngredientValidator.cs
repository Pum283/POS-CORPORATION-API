namespace API;

public class UpdateInventoryIngredientValidator : AbstractValidator<UpdateInventoryIngredientModel>
{
    public UpdateInventoryIngredientValidator()
    {
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .NotEqual(Guid.Empty).WithMessage(ErrorMessageBase.Empty);
        RuleFor(x => x.IngredientId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .NotEqual(Guid.Empty).WithMessage(ErrorMessageBase.Empty);
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThanOrEqual)
            .WithMessage(ErrorMessageBase.Required);
    }
}
