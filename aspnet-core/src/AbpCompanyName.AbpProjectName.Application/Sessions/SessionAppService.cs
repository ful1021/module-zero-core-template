using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Session;
using AbpCompanyName.AbpProjectName.Sessions.Dto;

namespace AbpCompanyName.AbpProjectName.Sessions
{
    public class SessionAppService : AbpProjectNameAppServiceBase, ISessionAppService
    {
        protected IUserNavigationManager UserNavigationManager { get; }
        protected ISettingDefinitionManager SettingDefinitionManager { get; }
        private readonly IIocResolver _iocResolver;

        public SessionAppService(IUserNavigationManager userNavigationManager, ISettingDefinitionManager settingDefinitionManager, IIocResolver iocResolver)
        {
            UserNavigationManager = userNavigationManager;
            SettingDefinitionManager = settingDefinitionManager;
            _iocResolver = iocResolver;
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
                output.Roles = role;

                output.Session = new
                {
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId,
                    ImpersonatorUserId = AbpSession.ImpersonatorUserId,
                    ImpersonatorTenantId = AbpSession.ImpersonatorTenantId,
                    MultiTenancySide = AbpSession.MultiTenancySide
                };

                output.GrantedPermissions = await GetGrantedPermissionNames();
            }

            return output;
        }

        protected async Task<List<string>> GetGrantedPermissionNames()
        {
            var allPermissionNames = PermissionManager.GetAllPermissions(false).Select(p => p.Name).ToList();
            var grantedPermissionNames = new List<string>();

            if (AbpSession.UserId.HasValue)
            {
                foreach (var permissionName in allPermissionNames)
                {
                    if (await PermissionChecker.IsGrantedAsync(permissionName))
                    {
                        grantedPermissionNames.Add(permissionName);
                    }
                }
            }

            return grantedPermissionNames;
        }

        //[AbpAuthorize]
        [DisableAuditing]
        public async Task<Dictionary<string, UserMenu>> GetMenusAsync()
        {
            var userMenus = await UserNavigationManager.GetMenusAsync(AbpSession.ToUserIdentifier());
            return userMenus.ToDictionary(userMenu => userMenu.Name, userMenu => userMenu);
        }

        //[AbpAuthorize]//会先访问此接口，如果需要授权的话，将导致前端返回401，会弹窗登陆
        [DisableAuditing]
        public async Task<UserMenu> GetMenuAsync(string menuName)
        {
            return await UserNavigationManager.GetMenuAsync(menuName, AbpSession.ToUserIdentifier());
        }

        [DisableAuditing]
        public async Task<Dictionary<string, string>> GetUserSettingConfig()
        {
            var config = new Dictionary<string, string>();

            var settingDefinitions = SettingDefinitionManager
                .GetAllSettingDefinitions();

            using (var scope = _iocResolver.CreateScope())
            {
                foreach (var settingDefinition in settingDefinitions)
                {
                    if (!await settingDefinition.ClientVisibilityProvider.CheckVisible(scope))
                    {
                        continue;
                    }

                    var settingValue = await SettingManager.GetSettingValueAsync(settingDefinition.Name);
                    config.Add(settingDefinition.Name, settingValue);
                }
            }

            return config;
        }
    }
}