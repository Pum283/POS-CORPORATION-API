namespace API;

public class CreateOrUpdateValidator : AbstractValidator<CreateOrUpdateStoreModel>
{
    public CreateOrUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required)
            .MaximumLength(1000)
            .WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required)
            .MaximumLength(1000)
            .WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required)
            .Matches(@"^(0|\+84)[0-9]{9,10}$")
            .WithMessage("Số điện thoại không hợp lệ. Phải bắt đầu bằng 0 hoặc +84 và có 10–11 chữ số.");

    //        .Matches(@"^0[0-9]{9}$")
    //        .WithMessage("Số điện thoại phải có 10 chữ số và bắt đầu bằng 0.");

    //        .Matches(@"^\d{9,11}$")
    //        .WithMessage("Số điện thoại chỉ được chứa chữ số và có độ dài từ 9 đến 11.");
    }
}
