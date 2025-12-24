namespace API;

public class CreateOrUpdateInventoryValidator : AbstractValidator<CreateOrUpdateInventoryModel>
{
    public CreateOrUpdateInventoryValidator()
    {
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .NotEqual(Guid.Empty).WithMessage(ErrorMessageBase.Empty);
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .NotEqual(Guid.Empty).WithMessage(ErrorMessageBase.Empty);
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThanOrEqual)
            .WithMessage(ErrorMessageBase.Required);
    }
}
