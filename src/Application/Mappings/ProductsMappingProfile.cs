namespace Application;

public class ProductsMappingProfile : Profile
{
    public ProductsMappingProfile()
    {
        CreateMap<ProductEntity, Product>().ReverseMap();
        CreateMap<ProductEntity, CreateProductModel>().ReverseMap();
        CreateMap<ProductEntity, ProductUpdateModel>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));;
        CreateMap<ProductEntity, DeleteProducModel>().ReverseMap();
    }
}
