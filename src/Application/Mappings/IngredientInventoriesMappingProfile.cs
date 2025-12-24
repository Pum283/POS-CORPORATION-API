namespace Application;

public class IngredientInventoriesMappingProfile : Profile
{
    public IngredientInventoriesMappingProfile()
    {
        CreateMap<UpdateInventoryIngredientModel, IngredientInventoryEntity>().ReverseMap();
    }
}
