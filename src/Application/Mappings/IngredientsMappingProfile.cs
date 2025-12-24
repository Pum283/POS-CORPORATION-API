namespace Application;

public class IngredientsMappingProfile : Profile
{
    public IngredientsMappingProfile()
    {
        CreateMap<Ingredient, IngredientEntity>().ReverseMap();
        CreateMap<CreateOrUpdateIngredientModel, IngredientEntity>().ReverseMap();

        CreateMap<AddIngredientForProductModel, ProductIngredientEntity>().ReverseMap();
    }
}
