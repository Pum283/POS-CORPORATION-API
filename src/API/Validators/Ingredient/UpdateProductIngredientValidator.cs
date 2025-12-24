namespace API;

public class UpdateProductIngredientValidator : AbstractValidator<UpdateProductIngredientModel>
{
    public UpdateProductIngredientValidator()
    {
        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);
    }
}
