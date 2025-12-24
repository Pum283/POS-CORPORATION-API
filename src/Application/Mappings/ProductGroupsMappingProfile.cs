namespace Application;

public class ProductGroupsMappingProfile : Profile
{
    public ProductGroupsMappingProfile()
    {
        CreateMap<ProductGroupEntity, ProductGroup>().ReverseMap();
        CreateMap<ProductGroupEntity, ProductGroupBasicModel>().ReverseMap();
        CreateMap<ProductGroupEntity, CreateOrUpdateProductGroupModel>().ReverseMap();
    }
}
