namespace Application;

public class LoyaltyPointsMappingProfile : Profile
{
    public LoyaltyPointsMappingProfile()
    {
        CreateMap<CreateLoyaltyPointModel, LoyaltyPointEntity>().ReverseMap();
    }
}
