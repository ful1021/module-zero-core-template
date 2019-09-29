using Abp.Organizations;
using AbpCompanyName.AbpProjectName.Authorization.Roles;
using AbpCompanyName.AbpProjectName.Authorization.Users;
using AutoMapper;

namespace AbpCompanyName.AbpProjectName.Organizations.Dto
{
    public class OrganizationUnitMapProfile : Profile
    {
        public OrganizationUnitMapProfile()
        {
            CreateMap<User, OrganizationUnitUserListDto>();
            CreateMap<Role, OrganizationUnitRoleListDto>();
            CreateMap<OrganizationUnit, OrganizationUnitDto>();
        }
    }
}