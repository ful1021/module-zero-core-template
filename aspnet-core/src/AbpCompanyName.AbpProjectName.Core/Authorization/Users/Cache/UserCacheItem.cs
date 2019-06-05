using Abp.AutoMapper;

namespace AbpCompanyName.AbpProjectName.Authorization.Users.Cache
{
    [AutoMapFrom(typeof(User))]
    public class UserCacheItem
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string FullName { get; set; }
    }
}