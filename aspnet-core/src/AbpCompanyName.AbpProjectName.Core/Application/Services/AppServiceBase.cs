using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;
using Abp.Runtime.Caching;

namespace Abp.Application.Services
{
    /// <summary>
    /// 服务 父类
    /// </summary>
    public abstract class AppServiceBase<TEntity, TPrimaryKey> : ApplicationService
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// 缓存Manager
        /// </summary>
        public ICacheManager CacheManager { get; set; }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        protected readonly IRepository<TEntity, TPrimaryKey> Repository;

        /// <summary>
        /// 父类构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        protected AppServiceBase(IRepository<TEntity, TPrimaryKey> repository)
        {
            Repository = repository;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        #region 查询

        /// <summary>
        /// Should apply paging if needed.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TEntity> ApplyPaging<TListInput>(IQueryable<TEntity> query, TListInput input)
        {
            //Try to use paging if available
            var pagedInput = input as IPagedResultRequest;
            if (pagedInput != null)
            {
                return query.PageBy(pagedInput);
            }

            //Try to limit query result if available
            var limitedInput = input as ILimitedResultRequest;
            if (limitedInput != null)
            {
                return query.Take(limitedInput.MaxResultCount);
            }

            //No paging
            return query;
        }

        /// <summary>
        /// Should apply sorting if needed.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TQueryDto> ApplySorting<TListInput, TQueryDto>(IQueryable<TQueryDto> query, TListInput input) where TQueryDto : IEntity<TPrimaryKey>
        {
            //Try to sort query if available
            var sortInput = input as ISortedResultRequest;
            if (sortInput != null)
            {
                if (!sortInput.Sorting.IsNullOrWhiteSpace())
                {
                    return query.OrderBy(sortInput.Sorting);
                }
            }

            //IQueryable.Task requires sorting, so we should sort if Take will be used.
            if (input is ILimitedResultRequest)
            {
                return query.OrderByDescending(e => e.Id);
            }

            //No sorting
            return query;
        }

        protected virtual async Task<PagedResultDto<TBasicEntityDto>> ToPagedList<TBasicEntityDto>(IQueryable<TEntity> query, IPagedResultRequest input, Func<TEntity, TBasicEntityDto> mapTo)
        {
            return await PagedList(query, input, mapTo: mapTo);
        }

        protected virtual async Task<PagedResultDto<TBasicEntityDto>> ToPagedList<TBasicEntityDto>(IQueryable<TEntity> query, IPagedResultRequest input)
        {
            return await PagedList<TBasicEntityDto>(query, input);
        }

        protected virtual async Task<PagedResultDto<TBasicEntityDto>> ToPagedList<TBasicEntityDto>(IQueryable<TEntity> query, IPagedResultRequest input, Func<IQueryable<TEntity>, IQueryable<TBasicEntityDto>> selectMap)
        {
            return await PagedList(query, input, selectMap: selectMap);
        }

        private async Task<PagedResultDto<TBasicEntityDto>> PagedList<TBasicEntityDto>(IQueryable<TEntity> query, IPagedResultRequest input, Func<IQueryable<TEntity>, IQueryable<TBasicEntityDto>> selectMap = null, Func<TEntity, TBasicEntityDto> mapTo = null)
        {
            var noPagerQuery = query;

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var list = await ToList(query, selectMap, mapTo, false);

            return await GetPagedResult(input, noPagerQuery, list);
        }

        protected async Task<List<TBasicEntityDto>> ToList<TBasicEntityDto>(IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TBasicEntityDto>> selectMap = null, Func<TEntity, TBasicEntityDto> mapTo = null, bool useAutoMapper = false)
        {
            List<TBasicEntityDto> result;

            //if (useAutoMapper)
            //{
            //    result = await AsyncQueryableExecuter.ToListAsync(query.ProjectTo<TBasicEntityDto>());
            //}
            //else
            {
                if (selectMap == null)
                {
                    var entities = await AsyncQueryableExecuter.ToListAsync(query);

                    if (mapTo == null)
                    {
                        result = ObjectMapper.Map<List<TBasicEntityDto>>(entities);
                    }
                    else
                    {
                        result = entities.Select(mapTo).ToList();
                    }
                }
                else
                {
                    result = await AsyncQueryableExecuter.ToListAsync(selectMap(query));
                }
            }

            return result;
        }

        protected virtual async Task<PagedResultDto<TBasicEntityDto>> GetPagedResult<TPagedListInput, TBasicEntityDto>(TPagedListInput input, IQueryable<TEntity> noPagerQuery, List<TBasicEntityDto> entities) where TPagedListInput : IPagedResultRequest
        {
            int totalCount = await AsyncQueryableExecuter.CountAsync(noPagerQuery);

            return new PagedResultDto<TBasicEntityDto>(totalCount, entities);
        }

        #endregion 查询

        #region 查询一条记录

        protected virtual async Task<TEntityDto> Get<TEntityDto>(EntityDto<TPrimaryKey> input) where TEntityDto : new()
        {
            var entity = await Repository.GetAsync(input.Id);
            return MapToEntityDto<TEntityDto>(entity);
        }

        protected virtual async Task<TDetailEntityDto> FirstOrDefaultAsync<TDetailEntityDto>(EntityDto<TPrimaryKey> input) where TDetailEntityDto : new()
        {
            //if (useAutoMapper)
            //{
            //    var query = Repository.GetAll().Where(CreateEqualityExpressionForId(input.Id));
            //    return await AsyncQueryableExecuter.FirstOrDefaultAsync(query.ProjectTo<TDetailEntityDto>());
            //}
            var info = await Repository.FirstOrDefaultAsync(input.Id);
            return MapToEntityDto<TDetailEntityDto>(info);
        }

        protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var leftExpression = Expression.PropertyOrField(lambdaParam, "Id");

            Expression<Func<object>> closure = () => id;
            var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

            var lambdaBody = Expression.Equal(leftExpression, rightExpression);

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        #endregion 查询一条记录

        #region MapTo

        protected virtual TEntityDto MapToEntityDto<TEntityDto>(TEntity entity) where TEntityDto : new()
        {
            if (entity == null)
            {
                return new TEntityDto();
            }
            return ObjectMapper.Map<TEntityDto>(entity);
        }

        protected virtual TEntity MapToEntity<TCreateInput>(TCreateInput createInput)
        {
            return ObjectMapper.Map<TEntity>(createInput);
        }

        protected virtual void MapToEntity<TUpdateInput>(TUpdateInput updateInput, TEntity entity)
        {
            ObjectMapper.Map(updateInput, entity);
        }

        #endregion MapTo

        #region 检查权限

        protected virtual void CheckPermission(string permissionName)
        {
            if (!string.IsNullOrEmpty(permissionName))
            {
                PermissionChecker.Authorize(permissionName);
            }
        }

        #endregion 检查权限
    }
}