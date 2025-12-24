namespace Application;

public class InventoryPeriodLockMappingProfile : Profile
{
    public InventoryPeriodLockMappingProfile()
    {
        CreateMap<UpdateInventoryQuantityLockModel, InventoryPeriodLockEntity>().ReverseMap();
        CreateMap<UpdateIngredientInventoryQuantityLockModel, InventoryPeriodLockEntity>().ReverseMap();
    }
}
