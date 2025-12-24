namespace API;

public class CreateOrUpdateOperationMetricModelValidator : AbstractValidator<CreateOrUpdateOperationMetricModel>
{
    public CreateOrUpdateOperationMetricModelValidator()
    {
        RuleFor(x => x.StoreId).NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.AppType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(AppType)).Contains(value));

        RuleFor(x => x.GMV).GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThanOrEqual);
        RuleFor(x => x.NetSale).GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThanOrEqual);
        RuleFor(x => x.OrderCount).GreaterThanOrEqualTo(0).WithMessage(ErrorMessageBase.GreaterThanOrEqual);
        RuleFor(x => x.RecordDate).NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
