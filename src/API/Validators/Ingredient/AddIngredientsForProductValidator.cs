namespace API;

public class AddIngredientsForProductValidator : AbstractValidator<AddIngredientForProductModel>
{
    public AddIngredientsForProductValidator()
    {
        RuleFor(x => x.ProductId)
            .NotNull().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.IngredientId)
            .NotNull().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);
    }
}
