namespace Application;

public class StoreAccountantsMappingProfile : Profile
{
    public StoreAccountantsMappingProfile()
    {
        CreateMap<StoreAccountantModel, StoreAccountantEntity>().ReverseMap();
        CreateMap<CreateOrUpdateStoreAccountantModel, StoreAccountantEntity>()
            .ForMember(dest => dest.StoreId, opt => opt.Ignore());
    }
}
