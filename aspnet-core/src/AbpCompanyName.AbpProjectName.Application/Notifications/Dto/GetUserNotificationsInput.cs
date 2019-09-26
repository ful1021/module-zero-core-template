using System;
using Abp.Notifications;
using AbpCompanyName.AbpProjectName.Dto;

namespace AbpCompanyName.AbpProjectName.Notifications.Dto
{
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}