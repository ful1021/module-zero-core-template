using Abp.Application.Navigation;
using AbpCompanyName.AbpProjectName.Authorization.Permissions;

namespace AbpCompanyName.AbpProjectName.Web.Host.Startup
{
    public class AppNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            var menu = context.Manager.MainMenu;
            BuilderUtils.BuildMenus("Permission", menu);
        }
    }
}