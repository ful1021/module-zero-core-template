using System.Threading.Tasks;
using Abp.Notifications;
using AbpCompanyName.AbpProjectName.Authorization.Users;

namespace AbpCompanyName.AbpProjectName.Notifications
{
    public class AppNotifier : AbpProjectNameDomainServiceBase
    {
        private readonly INotificationPublisher _notificationPublisher;

        public AppNotifier(INotificationPublisher notificationPublisher)
        {
            _notificationPublisher = notificationPublisher;
        }

        public async Task WelcomeToTheApplicationAsync(User user)
        {
            await _notificationPublisher.PublishAsync(
                AppNotificationNames.WelcomeToTheApplication,
                new MessageNotificationData("欢迎来到我们的应用 !通知系统是用来告诉您事件。您可以选择您想要哪种类型的通知收到通知设置。"),
                severity: NotificationSeverity.Success,
                userIds: new[] { user.ToUserIdentifier() }
                );
        }
    }
}