namespace Application;

public class CustomersMappingProfile : Profile
{
    public CustomersMappingProfile()
    {
        CreateMap<CustomerEntity, Customer>().ReverseMap();
        CreateMap<CustomerEntity, CreateOrUpdateCustomerModel>().ReverseMap();
    }
}
