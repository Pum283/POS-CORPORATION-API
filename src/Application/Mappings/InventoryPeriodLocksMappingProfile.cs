namespace Application;

public class InventoryPeriodLocksMappingProfile : Profile
{
    public InventoryPeriodLocksMappingProfile()
    {
        CreateMap<InventoryPeriodLockModel, InventoryPeriodLockEntity>().ReverseMap();
    }
}
