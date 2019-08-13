using System.Threading.Tasks;
using Abp;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Timing;
using AbpCompanyName.AbpProjectName.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AbpCompanyName.AbpProjectName.Web.Host.Controllers
{
    public class HomeController : AbpProjectNameControllerBase
    {
        private readonly IHostingEnvironment _env;
        private readonly INotificationPublisher _notificationPublisher;

        public HomeController(INotificationPublisher notificationPublisher, IHostingEnvironment env)
        {
            _notificationPublisher = notificationPublisher;
            _env = env;
        }

        public IActionResult Index()
        {
            if (_env.IsDevelopment())
            {
                return Redirect("/swagger");
            }
            else
            {
                return Json(new { _env.EnvironmentName });
            }
        }

        /// <summary>
        /// This is a demo code to demonstrate sending notification to default tenant admin and host admin uers.
        /// Don't use this code in production !!!
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ActionResult> TestNotification(string message = "")
        {
            if (message.IsNullOrEmpty())
            {
                message = "This is a test notification, created at " + Clock.Now;
            }

            var defaultTenantAdmin = new UserIdentifier(1, 2);
            var hostAdmin = new UserIdentifier(null, 1);

            await _notificationPublisher.PublishAsync(
                "App.SimpleMessage",
                new MessageNotificationData(message),
                severity: NotificationSeverity.Info,
                userIds: new[] { defaultTenantAdmin, hostAdmin }
            );

            return Content("Sent notification: " + message);
        }
    }
}