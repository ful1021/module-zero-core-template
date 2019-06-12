using Abp.Application.Navigation;
using Abp.Localization;
using AbpCompanyName.AbpProjectName.Authorization;

namespace AbpCompanyName.AbpProjectName.Web.Host.Startup
{
    public class AppNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            var menu = context.Manager.MainMenu;

            menu.AddItem(BuildSystemMenu())
                ;
        }

        private static MenuItemDefinition BuildSystemMenu()
        {
            return NewMenuItem(AppPermissions.System, "设置", "/system", "lock")
                .AddItem(NewMenuItem(AppPermissions.System_Roles, "角色管理", "role"))
                .AddItem(NewMenuItem(AppPermissions.System_Users, "用户管理", "user"));
        }

        private static MenuItemDefinition NewMenuItem(string name, string displayName, string url = null, string icon = null)
        {
            return new MenuItemDefinition(name, new FixedLocalizableString(displayName), icon, url, requiredPermissionName: name);
        }
    }
}