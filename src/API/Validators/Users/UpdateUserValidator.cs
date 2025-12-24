namespace API;

public class UpdateUserValidator : AbstractValidator<UpdateUserModel>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Length(3, 256).WithMessage(ErrorMessageBase.Range);
        //RuleFor(x => x.Picture)
        RuleFor(x => x.Phone)
            .Matches(@"^(0|\+84)[0-9]{9,10}$")
            .WithMessage("Số điện thoại không hợp lệ. Phải bắt đầu bằng 0 hoặc +84 và có 10–11 chữ số.");
    }
}
