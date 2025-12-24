namespace Application;

public class UsersMappingProfile : Profile
{
    public UsersMappingProfile()
    {
        CreateMap<UserModel, UserEntity>().ReverseMap();
        CreateMap<User, UserEntity>().ReverseMap();
        CreateMap<UpdateUserModel, User>().ReverseMap();
        CreateMap<UpdateUserModel, UserEntity>()
            .ForMember(dest => dest.Picture, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.Picture, opt => opt.Ignore());
        CreateMap<CreateUserModel, User>().ReverseMap();
        CreateMap<CreateUserModel, UserEntity>().ReverseMap();
        CreateMap<UserForTokenModel, UserEntity>().ReverseMap();
        CreateMap<UserForTokenModel, User>().ReverseMap();
    }
}
