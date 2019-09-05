using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Auditing;
using Abp.Authorization;
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
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());

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

        [AbpAuthorize]
        [DisableAuditing]
        public async Task<Dictionary<string, UserMenu>> GetMenusAsync()
        {
            var userMenus = await UserNavigationManager.GetMenusAsync(AbpSession.ToUserIdentifier());
            return userMenus.ToDictionary(userMenu => userMenu.Name, userMenu => userMenu);
        }

        [AbpAuthorize]
        [DisableAuditing]
        public async Task<UserMenu> GetMenuAsync(string menuName)
        {
            return await UserNavigationManager.GetMenuAsync(menuName, AbpSession.ToUserIdentifier());
        }
    }
}