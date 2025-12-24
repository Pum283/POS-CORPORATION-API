namespace API;

public class CreateCreateSalesOrderModelValidator : AbstractValidator<CreateSalesOrderModel>
{
    public CreateCreateSalesOrderModelValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(PaymentMethodType)).Contains(value)).WithMessage(ErrorMessageBase.InvalidFormat);
            
        RuleFor(x => x.ShippingType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)  
            .Must(value => Enum.GetNames(typeof(ShippingType)).Contains(value)).WithMessage(ErrorMessageBase.InvalidFormat);

        RuleFor(x => x.SalesOrderItem)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(items => items!.Count > 0).WithMessage(ErrorMessageBase.Required);

    }
}
