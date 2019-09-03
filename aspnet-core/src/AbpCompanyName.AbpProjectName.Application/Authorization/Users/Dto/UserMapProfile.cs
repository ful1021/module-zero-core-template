using AutoMapper;

namespace AbpCompanyName.AbpProjectName.Authorization.Users.Dto
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<UserDto, User>()
                .ForMember(x => x.Roles, opt => opt.Ignore())
                .ForMember(x => x.CreationTime, opt => opt.Ignore());

            CreateMap<UserCreateInput, User>();
            CreateMap<UserCreateInput, User>().ForMember(x => x.Roles, opt => opt.Ignore());

            CreateMap<UserUpdateInput, User>();
            CreateMap<UserUpdateInput, User>().ForMember(x => x.Roles, opt => opt.Ignore());

            CreateMap<User, UserDto>();
            CreateMap<User, UserListDto>();
        }
    }
}