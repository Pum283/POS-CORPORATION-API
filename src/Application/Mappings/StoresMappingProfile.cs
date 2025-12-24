namespace Application;

public class StoresMappingProfile : Profile
{
    public StoresMappingProfile()
    {
        CreateMap<StoreEntity, Store>().ReverseMap();
        CreateMap<CreateOrUpdateStoreModel, StoreEntity>().ReverseMap();
        CreateMap<StoreEntity, CreateOrUpdateStoreModel>().ReverseMap();
    }
}
