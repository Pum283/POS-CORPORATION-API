namespace Application;

public class PurchaseOrdersMappingProfile : Profile
{
    public PurchaseOrdersMappingProfile()
    {
        CreateMap<CreatePurchaseOrderModel, PurchaseOrderEntity>().ReverseMap();
        CreateMap<CreatePurchaseOrderItem, PurchaseOrderItemEntity>().ReverseMap();
    }
}
