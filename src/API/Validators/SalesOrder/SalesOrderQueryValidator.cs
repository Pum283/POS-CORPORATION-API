namespace API;

public class SalesOrderQueryValidator : AbstractValidator<SalesOrderQuery>
{
    public SalesOrderQueryValidator()
    {
        RuleFor(x => x.ShippingType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(ShippingType)).Contains(value))
            .WithMessage(ErrorMessageBase.InvalidFormat);
    }
}
