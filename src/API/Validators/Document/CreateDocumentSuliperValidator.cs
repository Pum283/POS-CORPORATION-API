namespace API;

public class CreateDocumentSuliperValidator : AbstractValidator<CreateDocumentSupplierModel>
{
    public CreateDocumentSuliperValidator()
    {
        RuleFor(x => x.TransType)
            .NotEmpty()
            .WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(TransTypeStaff)).Contains(value))
            .WithMessage(ErrorMessageBase.InvalidFormat);
    }
}
