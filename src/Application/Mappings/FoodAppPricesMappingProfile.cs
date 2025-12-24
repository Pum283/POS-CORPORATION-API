// File: Application/FoodAppPricesMappingProfile.cs
namespace Application;

public class FoodAppPricesMappingProfile : Profile
{
    public FoodAppPricesMappingProfile()
    {
        CreateMap<CreateFoodAppPriceModel, FoodAppPriceEntity>();
        CreateMap<UpdateFoodAppPriceModel, FoodAppPriceEntity>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Không cần map ngược vì bạn select thủ công trong GetAllWithPagingAsync
        // Nhưng nếu muốn dùng _mapper.Map<FoodAppPriceModel>(entity) thì thêm dòng dưới:
        CreateMap<FoodAppPriceEntity, FoodAppPriceModel>();
    }
}