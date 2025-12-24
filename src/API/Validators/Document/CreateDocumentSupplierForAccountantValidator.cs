namespace API;

public class CreateDocumentSupplierForAccountantValidator : AbstractValidator<CreateDocumentSupplierForAccountantModel>
{
    public CreateDocumentSupplierForAccountantValidator()
    {
        RuleFor(x => x.TransType)
           .NotEmpty()
           .WithMessage(ErrorMessageBase.Required)
           .Must(value => Enum.GetNames(typeof(TransTypeAccountant)).Contains(value))
           .WithMessage(ErrorMessageBase.InvalidFormat);
    }
}
