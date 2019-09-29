using Abp.Authorization;
using Abp.Localization;
using Abp.Notifications;
using AbpCompanyName.AbpProjectName.Authorization;

namespace AbpCompanyName.AbpProjectName.Notifications
{
    public class AppNotificationProvider : NotificationProvider
    {
        public override void SetNotifications(INotificationDefinitionContext context)
        {
            context.Manager.Add(
                new NotificationDefinition(
                    AppNotificationNames.NewUserRegistered,
                    displayName: L("NewUserRegisteredNotificationDefinition"),
                    permissionDependency: new SimplePermissionDependency(PermissionNames.System_Users)
                    )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpProjectNameConsts.LocalizationSourceName);
        }
    }
}