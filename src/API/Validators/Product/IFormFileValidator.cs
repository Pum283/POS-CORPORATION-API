namespace API;

public class IFormFileValidator : AbstractValidator<IFormFile>
{
    public IFormFileValidator()
    {
        RuleFor(file => file)
            .NotNull().WithMessage(ErrorMessageBase.Required);
            //.Must(FileValidatorHelper.IsValidExtension)
            //    .WithMessage(ErrorMessageBase.InvalidSizeImage);
            //.Must(file => FileValidatorHelper.IsValidSize(file, 2 * 1024 * 1024))
            //    .WithMessage(ErrorMessageBase.InvalidSizeImage);
    }
}
