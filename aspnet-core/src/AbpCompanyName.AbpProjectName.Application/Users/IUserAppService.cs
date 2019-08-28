using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AbpCompanyName.AbpProjectName.Roles.Dto;
using AbpCompanyName.AbpProjectName.Users.Dto;

namespace AbpCompanyName.AbpProjectName.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}