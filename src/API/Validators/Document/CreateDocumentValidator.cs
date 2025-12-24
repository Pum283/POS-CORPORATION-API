namespace API;

public class CreateDocumentValidator : AbstractValidator<CreateDocumentModel>
{
    public CreateDocumentValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength);
    }
}
