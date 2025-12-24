namespace API;

public class OperationMetricQueryValidator : AbstractValidator<OperationMetricQuery>
{
    public OperationMetricQueryValidator()
    {
        RuleFor(x => x.StoreId).NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
