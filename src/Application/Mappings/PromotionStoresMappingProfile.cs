namespace Application;

public class PromotionStoresMappingProfile : Profile
{
    public PromotionStoresMappingProfile()
    {
        CreateMap<UpdatePromotionStoreModel, PromotionStoreEntity>().ReverseMap();
    }
}
