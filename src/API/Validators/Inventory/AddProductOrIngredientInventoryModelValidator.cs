namespace API;

public class AddProductOrIngredientInventoryModelValidator : AbstractValidator<AddProductOrIngredientInventoryModel>
{
    public AddProductOrIngredientInventoryModelValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);
    }
}
