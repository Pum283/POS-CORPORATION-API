namespace API;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusModel>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .NotEqual(Guid.Empty).WithMessage(ErrorMessageBase.Empty);
        RuleFor(x => x.OrderStatus)
            .NotEmpty().WithMessage(ErrorMessageBase.Empty)
            .Must(value => Enum.GetNames(typeof(OrderStatusType)).Contains(value))
            .WithMessage(ErrorMessageBase.InvalidFormat);
        RuleFor(x => x.IsUsingPoints)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .Must(isUsingPoints => isUsingPoints == true || isUsingPoints == false)
            .WithMessage("IsUsingPoints must be a boolean value (true or false).");
    }
}
