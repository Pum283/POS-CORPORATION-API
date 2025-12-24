namespace Application;

public class DocumentsMappingProfile : Profile
{
    public DocumentsMappingProfile()
    {
        CreateMap<CreateDocumentModel, DocumentEntity>().ReverseMap();

        CreateMap<AddProductOrIngredientInventoryModel, DocumentSupplierEntity>()
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

        CreateMap<CreateDocumentSupplierForAccountantModel, CreateDocumentSupplierModel>().ReverseMap();
        CreateMap<AddProductOrIngredientInventoryWhenDocumentExportrotationModel, DocumentSupplierEntity>()
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ReverseMap();
    }
}
