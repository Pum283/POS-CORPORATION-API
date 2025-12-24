namespace API;

public class CreateInventoryValidator : AbstractValidator<CreateOrUpdateInventoryModel>
{
    public CreateInventoryValidator()
    {
        RuleFor(x => x.StoreId).NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.ProductId).NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThanOrEqual);
    }
}