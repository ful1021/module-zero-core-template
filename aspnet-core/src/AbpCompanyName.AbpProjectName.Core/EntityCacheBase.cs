using System;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Caching;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;

namespace AbpCompanyName.AbpProjectName
{
    public class EntityCacheBase<TEntity, TCacheItem> : EntityCacheBase<TEntity, TCacheItem, int>, IEntityCache<TCacheItem>
    where TEntity : class, IEntity<int>
    {
        public EntityCacheBase(ICacheManager cacheManager, IRepository<TEntity, int> repository, string cacheName = null) : base(cacheManager, repository, cacheName)
        {
        }
    }

    public class EntityCacheBase<TEntity, TCacheItem, TPrimaryKey> : EntityCache<TEntity, TCacheItem, TPrimaryKey>, IEventHandler<EntityChangedEventData<TEntity>>, IEntityCache<TCacheItem, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
    {
        public EntityCacheBase(ICacheManager cacheManager, IRepository<TEntity, TPrimaryKey> repository, string cacheName = null) : base(cacheManager, repository, cacheName)
        {
        }

        #region TCacheItem

        public override TCacheItem Get(TPrimaryKey id)
        {
            InternalCache.DefaultSlidingExpireTime = TimeSpan.FromDays(100);
            return base.Get(id);
        }

        public override Task<TCacheItem> GetAsync(TPrimaryKey id)
        {
            InternalCache.DefaultSlidingExpireTime = TimeSpan.FromDays(100);
            return base.GetAsync(id);
        }

        #endregion TCacheItem

        public override void HandleEvent(EntityChangedEventData<TEntity> eventData)
        {
            InternalCache.Remove(eventData.Entity.Id);
        }
    }
}