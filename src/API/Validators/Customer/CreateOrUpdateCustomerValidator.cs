namespace API;
public class CreateOrUpdateCustomerValidator : AbstractValidator<CreateOrUpdateCustomerModel>
{
    public CreateOrUpdateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.Phone)
            .Matches(@"^\S+$").WithMessage(ErrorMessageBase.NotContainSpaces)
            .Matches(@"^(0|\+84)[0-9]{9,10}$")
            .WithMessage("Phone number is invalid. It must start with 0 or +84 and be 10 to 11 digits long.");
    }
}
