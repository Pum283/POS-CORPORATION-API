namespace API;

public class GetInventoryReportValidator : AbstractValidator<GetInventoryReportQuery>
{
    public GetInventoryReportValidator()
    {
        //RuleFor(x => x.Unit)
        //    .Must(value => Enum.GetNames(typeof(ShippingType)).Contains(value))
        //    .WithMessage(ErrorMessageBase.InvalidFormat);
    }
}
