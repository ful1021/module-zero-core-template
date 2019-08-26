using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Auditing;
using Abp.Runtime.Session;
using AbpCompanyName.AbpProjectName.Sessions.Dto;

namespace AbpCompanyName.AbpProjectName.Sessions
{
    public class SessionAppService : AbpProjectNameAppServiceBase, ISessionAppService
    {
        protected IUserNavigationManager UserNavigationManager { get; }

        public SessionAppService(IUserNavigationManager userNavigationManager)
        {
            UserNavigationManager = userNavigationManager;
        }

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
                output.Roles = role;

                output.Session = new
                {
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId,
                    ImpersonatorUserId = AbpSession.ImpersonatorUserId,
                    ImpersonatorTenantId = AbpSession.ImpersonatorTenantId,
                    MultiTenancySide = AbpSession.MultiTenancySide
                };
            }

            return output;
        }

        [DisableAuditing]
        public async Task<Dictionary<string, UserMenu>> GetMenusAsync()
        {
            var userMenus = await UserNavigationManager.GetMenusAsync(AbpSession.ToUserIdentifier());
            return userMenus.ToDictionary(userMenu => userMenu.Name, userMenu => userMenu);
        }

        [DisableAuditing]
        public async Task<UserMenu> GetMenuAsync(string menuName)
        {
            return await UserNavigationManager.GetMenuAsync(menuName, AbpSession.ToUserIdentifier());
        }
    }
}