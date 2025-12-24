namespace Application;

public class PromotionsMappingProfile : Profile
{
    public PromotionsMappingProfile()
    {
        CreateMap<CreateSalesOrderPromotionModel, SalesOrderPromotionEntity>().ReverseMap();
        CreateMap<Promotion, PromotionEntity>().ReverseMap();
        CreateMap<CreatePromotionModel, PromotionEntity>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<PromotionType>(src.PromotionType!)))
            .ForMember(dest => dest.ProductScope, opt => opt.MapFrom(src => Enum.Parse<ProductTargetScopeType>(src.ProductScope!)))
            .ForMember(dest => dest.StoreScope, opt => opt.MapFrom(src => Enum.Parse<StoreTargetScopeType>(src.StoreScope!)))
            .ReverseMap();
        CreateMap<UpdatePromotionModel, PromotionEntity>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<PromotionType>(src.PromotionType!)))
            //.ForMember(dest => dest.ProductScope, opt => opt.MapFrom(src => Enum.Parse<ProductTargetScopeType>(src.ProductScope!)))
            //.ForMember(dest => dest.StoreScope, opt => opt.MapFrom(src => Enum.Parse<StoreTargetScopeType>(src.StoreScope!)))
            .ReverseMap();
    }
}
