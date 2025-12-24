public class CreatePromotionStoreModelValidator : AbstractValidator<CreatePromotionStoreModel>
{
    public CreatePromotionStoreModelValidator()
    {
        RuleFor(x => x)
           .Custom((model, context) =>
           {
               var count = 0;

               if (model.StoreId.HasValue && model.StoreId.Value != Guid.Empty)
                   count++;
               if (model.ProductId.HasValue && model.ProductId.Value != Guid.Empty)
                   count++;
               if (model.ProductGroupId.HasValue && model.ProductGroupId.Value != Guid.Empty)
                   count++;

               //if (count == 0)
               //{
               //    context.AddFailure("At least one of StoreId, ProductId, or ProductGroupId must be provided.");
               //}
               //else

               if (count > 1)
               {
                   context.AddFailure("Only one of StoreId, ProductId, or ProductGroupId should be provided.");
               }
           });

        RuleFor(x => x.CreatePromotionModel)
            .NotNull().WithMessage("CreatePromotionModel is required.")
            .SetValidator(new CreatePromotionModelValidator());
    }
}
