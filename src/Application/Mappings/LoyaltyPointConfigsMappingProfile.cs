// File: Application/LoyaltyPointConfigsMappingProfile.cs
namespace Application;

public class LoyaltyPointConfigsMappingProfile : Profile
{
    public LoyaltyPointConfigsMappingProfile()
    {
        CreateMap<CreateLoyaltyPointConfigModel, LoyaltyPointConfigEntity>();
        CreateMap<UpdateLoyaltyPointConfigModel, LoyaltyPointConfigEntity>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<LoyaltyPointConfigEntity, LoyaltyPointConfigModel>();
    }
}