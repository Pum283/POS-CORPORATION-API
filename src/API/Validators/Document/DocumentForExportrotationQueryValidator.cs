namespace API;

public class DocumentForExportrotationQueryValidator : AbstractValidator<DocumentForExportrotationQuery>
{
    public DocumentForExportrotationQueryValidator()
    {
        RuleFor(x => x.DocumentStatusType)
            .Must(value => Enum.IsDefined(typeof(DocumentStatusType), value))
            .WithMessage("Invalid DocumentStatusType value.");
    }
}
