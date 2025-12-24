namespace Application;

public class InventoriesMappingProfile : Profile
{
    public InventoriesMappingProfile()
    {
        CreateMap<InventoryEntity, InventoryViewModel>().ReverseMap();
        CreateMap<InventoryEntity, CreateOrUpdateInventoryModel>().ReverseMap();
    }
}
