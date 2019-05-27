using Abp.Application.Navigation;
using Abp.Localization;

namespace AbpCompanyName.AbpProjectName.Web.Host.Startup.Nav
{
    public class AppNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            var menu = context.Manager.MainMenu;

            menu.AddItem(new MenuItemDefinition(
                "System"
                , new FixedLocalizableString("设置")
                , "lock"
                , "/system"
                )
                .AddItem(new MenuItemDefinition(
                    "RoleManager"
                    , new FixedLocalizableString("角色管理")
                    , url: "role"
                ))
                .AddItem(new MenuItemDefinition(
                    "UserManager"
                    , new FixedLocalizableString("用户管理")
                    , url: "user"
                ))

                );
        }
    }
}