namespace Application;

public class SalesOrdersMappingProfile : Profile
{
    public SalesOrdersMappingProfile()
    {
        // Sales Order
        CreateMap<CreateSalesOrderModel, SalesOrderEntity>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => ParsePaymentMethod(src.PaymentMethod)))
            .ForMember(dest => dest.SalesOrderPromotion, opt => opt.Ignore())
            .ReverseMap();

        //Sales Order Item
        CreateMap<CreateSalesOrderItemModel, SalesOrderItemEntity>().ReverseMap();
        CreateMap<SalesOrder, SalesOrderEntity>().ReverseMap();
    }
    private static PaymentMethodType ParsePaymentMethod(string? paymentMethod)
    {
        return Enum.TryParse<PaymentMethodType>(paymentMethod, out var unitType) ? unitType : PaymentMethodType.Other;
    }
}
