using Abp.AutoMapper;
using AbpCompanyName.AbpProjectName.Authorization.Roles;

namespace AbpCompanyName.AbpProjectName.Authorization.Roles.Cache
{
    [AutoMapFrom(typeof(Role))]
    public class RoleCacheItem
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool IsStatic { get; set; }

        public bool IsDefault { get; set; }
    }
}