namespace API;

public class UpdateIngredientInventoryQuantityLockValidator : AbstractValidator<UpdateIngredientInventoryQuantityLockModel>
{
    public UpdateIngredientInventoryQuantityLockValidator()
    {
        RuleFor(x => x.IngredientId)
       .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.ClosingQuantity)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThanOrEqual);
    }
}