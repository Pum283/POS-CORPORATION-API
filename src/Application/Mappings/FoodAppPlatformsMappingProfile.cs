// File: Application/FoodAppPlatformsMappingProfile.cs
namespace Application;

public class FoodAppPlatformsMappingProfile : Profile
{
    public FoodAppPlatformsMappingProfile()
    {
        CreateMap<CreateFoodAppPlatformModel, FoodAppPlatformEntity>();
        CreateMap<UpdateFoodAppPlatformModel, FoodAppPlatformEntity>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<FoodAppPlatformEntity, FoodAppPlatformModel>();
    }
}