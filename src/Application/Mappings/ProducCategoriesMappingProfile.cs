namespace Application;

public class ProducCategoriesMappingProfile : Profile
{
    public ProducCategoriesMappingProfile()
    {
        CreateMap<ProductCategoryEntity, ProductCategory>().ReverseMap();
        CreateMap<ProductCategoryEntity, CreateOrUpdateProductCategoryModel>().ReverseMap();
        CreateMap<ProductCategoryEntity, ProductCategoryGroupModel>().ReverseMap();
    }
}
