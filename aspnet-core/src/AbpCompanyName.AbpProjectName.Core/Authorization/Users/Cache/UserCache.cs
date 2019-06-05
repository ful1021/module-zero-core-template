using Abp.Dependency;
using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;

namespace AbpCompanyName.AbpProjectName.Authorization.Users.Cache
{
    public class UserCache : EntityCacheBase<User, UserCacheItem, long>, ITransientDependency
    {
        public UserCache(ICacheManager cacheManager, IRepository<User, long> repository, string cacheName = null) : base(cacheManager, repository, cacheName)
        {
        }
    }
}