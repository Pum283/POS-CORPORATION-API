namespace API;

public class UpdateInventoryQuantityLockValidator : AbstractValidator<UpdateInventoryQuantityLockModel>
{
    public UpdateInventoryQuantityLockValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.ClosingQuantity)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThanOrEqual);
    }
}
