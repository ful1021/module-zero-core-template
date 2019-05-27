using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Auditing;
using AbpCompanyName.AbpProjectName.Sessions.Dto;

namespace AbpCompanyName.AbpProjectName.Sessions
{
    public class SessionAppService : AbpProjectNameAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>()
                }
            };

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper.Map<TenantLoginInfoDto>(await GetCurrentTenantAsync());
            }

            if (AbpSession.UserId.HasValue)
            {
                var user = await GetCurrentUserAsync();
                var role = await UserManager.GetRolesAsync(user);

                output.User = ObjectMapper.Map<UserLoginInfoDto>(user);

                output.Avatar = "//hbimg.huabanimg.com/9fd86985ec36de14e4a4040b2008f6d0df93515618f4-wm0Y66_fw658";// user.ProfilePicture;
                output.Name = user.Name;
                output.Introduction = user.Introduction;
                output.Roles = role;
            }

            return output;
        }
    }
}