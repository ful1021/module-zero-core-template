using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;

namespace AbpCompanyName.AbpProjectName.Authorization.Roles.Cache
{
    public class RoleCache : EntityCacheBase<Role, RoleCacheItem>, ITransientDependency
    {
        public RoleCache(ICacheManager cacheManager, IRepository<Role, int> repository, string cacheName = null) : base(cacheManager, repository, cacheName)
        {
        }
    }
}