namespace API;

public class CreatePurchaseOrderValidator : AbstractValidator<CreatePurchaseOrderModel>
{
    public CreatePurchaseOrderValidator()
    {
        RuleFor(x => x.TotalAmount)
           .NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
