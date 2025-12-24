namespace Application;

public class OperationMetricsMappingProfile : Profile
{
    public OperationMetricsMappingProfile()
    {
        CreateMap<CreateOrUpdateOperationMetricModel, OperationMetricEntity>()
            .ForMember(dest => dest.AppType, opt => opt.MapFrom(src => Enum.Parse<AppType>(src.AppType!)))
            .ReverseMap();  
    }
}
