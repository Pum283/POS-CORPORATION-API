namespace API;

public class UpdateUserByAdminValidator : AbstractValidator<UpdateUserByAdminModel>
{
    public UpdateUserByAdminValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage(ErrorMessageBase.Required);

        //RuleFor(x => x.StoreId)
        //    .NotEmpty().WithMessage(ErrorMessageBase.Required);
    }
}
