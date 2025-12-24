namespace API;

public class AddProductOrIngredientInventoryWhenDocumentExportrotationModelValidator : AbstractValidator<AddProductOrIngredientInventoryWhenDocumentExportrotationModel>
{
    public AddProductOrIngredientInventoryWhenDocumentExportrotationModelValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.IsProduct).NotNull().WithMessage(ErrorMessageBase.Required);
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);
    }
}
