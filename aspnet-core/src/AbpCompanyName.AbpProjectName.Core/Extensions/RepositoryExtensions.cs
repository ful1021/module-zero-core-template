using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.Domain.Repositories
{
    public static class RepositoryExtensions
    {
        public static async Task BatchDeleteAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, ICollection<TEntity> data) where TEntity : class, IEntity<TPrimaryKey>
        {
            foreach (var item in data)
            {
                await repository.DeleteAsync(item);
                //delete(item);
            }
        }
        public static async Task BatchInsertOrDeleteAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, ICollection<TEntity> oldData, ICollection<TEntity> newData, Func<ICollection<TEntity>, TEntity, bool> deleteCondition, Func<ICollection<TEntity>, TEntity, bool> insertCondition) where TEntity : class, IEntity<TPrimaryKey>
        {
            foreach (var item in oldData)
            {
                //.Where(p => !newData.Contains(p))
                if (deleteCondition(newData, item))
                {
                    await repository.DeleteAsync(item);
                    //delete(item);
                }
            }

            foreach (var item in newData)
            {
                //.Where(p => !oldData.Contains(p))
                if (insertCondition(oldData, item))
                {
                    await repository.InsertAsync(item);
                    //insert(item);
                }
            }
        }
    }
}