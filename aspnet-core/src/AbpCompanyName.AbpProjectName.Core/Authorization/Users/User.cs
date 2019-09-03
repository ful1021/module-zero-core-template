using System;
using System.Collections.Generic;
using Abp.Authorization.Users;
using Abp.Extensions;
using AbpCompanyName.AbpProjectName.Enumerations;

namespace AbpCompanyName.AbpProjectName.Authorization.Users
{
    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }

        public void Unlock()
        {
            AccessFailedCount = 0;
            LockoutEndDateUtc = null;
        }
    }

    [EnumData]
    public enum OrderStatus
    {
        待处理 = 0,
        处理中 = 1,
        已完成 = 3,
        已作废 = 2,
    }
}