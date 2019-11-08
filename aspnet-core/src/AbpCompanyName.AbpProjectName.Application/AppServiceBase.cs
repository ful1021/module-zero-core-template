using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq;
using Abp.Linq.Extensions;
using AutoMapper.QueryableExtensions;

namespace AbpCompanyName.AbpProjectName
{
    /// <summary>
    /// 服务 父类
    /// </summary>
    public abstract class AppServiceBase<TEntity, TPrimaryKey> : ApplicationService
        where TEntity : class, IEntity<TPrimaryKey>
    {
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
        protected virtual IQueryable<TEntity> ApplySorting<TListInput>(IQueryable<TEntity> query, TListInput input)
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

        protected virtual async Task<PagedResultDto<TBasicEntityDto>> ToPagedList<TPagedListInput, TBasicEntityDto>(IQueryable<TEntity> query, TPagedListInput input, Func<TEntity, TBasicEntityDto> selector = null) where TPagedListInput : IPagedResultRequest
        {
            var noPagerQuery = query;
            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await ToList(query, selector);

            int totalCount = entities?.Count ?? 0;
            var pagedInput = input as IPagedResultRequest;
            //没有分页参数,不需要统计总记录数  或者 总条数 小于 每页大小，则代表只有1页数据
            if (pagedInput != null || totalCount < pagedInput.MaxResultCount)
            {
                totalCount = await AsyncQueryableExecuter.CountAsync(noPagerQuery);
            }

            return new PagedResultDto<TBasicEntityDto>(totalCount, entities);
        }

        protected virtual async Task<ListResultDto<TBasicEntityDto>> ToList<TListInput, TBasicEntityDto>(IQueryable<TEntity> query, TListInput input = default(TListInput), Func<TEntity, TBasicEntityDto> selector = null)
        {
            if (input != null)
            {
                query = ApplySorting(query, input);
            }

            var entities = await ToList(query, selector);

            return new ListResultDto<TBasicEntityDto>(entities);
        }

        protected virtual async Task<List<TBasicEntityDto>> ToList<TBasicEntityDto>(IQueryable<TEntity> query, Func<TEntity, TBasicEntityDto> selector = null)
        {
            if (UserProjectTo)
            {
                return await AsyncQueryableExecuter.ToListAsync(ProjectToList<TBasicEntityDto>(query));
            }
            var entities = await AsyncQueryableExecuter.ToListAsync(query);
            if (selector == null)
            {
                return entities.Select(entity => ObjectMapper.Map<TBasicEntityDto>(entity)).ToList();
            }
            else
            {
                return entities.Select(selector).ToList();
            }
        }

        #endregion 查询

        #region 查询一条记录

        protected virtual async Task<TEntityDto> Get<TEntityDto>(EntityDto<TPrimaryKey> input)
        {
            var entity = await Repository.GetAsync(input.Id);
            return MapToEntityDto<TEntityDto>(entity);
        }

        protected virtual async Task<TDetailEntityDto> FirstOrDefaultAsync<TDetailEntityDto>(EntityDto<TPrimaryKey> input)
        {
            if (UserProjectTo)
            {
                var query = Repository.GetAll().Where(CreateEqualityExpressionForId(input.Id));
                return await AsyncQueryableExecuter.FirstOrDefaultAsync(ProjectTo<TDetailEntityDto>(query));
            }
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

        protected virtual TDetailEntityDto MapToEntityDto<TDetailEntityDto>(TEntity entity)
        {
            return ObjectMapper.Map<TDetailEntityDto>(entity);
        }

        protected virtual TEntity MapToEntity<TCreateInput>(TCreateInput createInput)
        {
            return ObjectMapper.Map<TEntity>(createInput);
        }

        protected virtual void MapToEntity<TUpdateInput>(TUpdateInput updateInput, TEntity entity)
        {
            ObjectMapper.Map(updateInput, entity);
        }

        /// <summary>
        /// 将实体模型中的每个元素映射到 TEntityQueryDto
        /// </summary>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, TBasicEntityDto>> SelectMapToList<TBasicEntityDto>()
        {
            return null;
        }

        /// <summary>
        /// 将实体模型中的每个元素映射到 TEntityQueryDto
        /// </summary>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, TDetailEntityDto>> SelectMapTo<TDetailEntityDto>()
        {
            return null;
        }

        protected virtual IQueryable<TBasicEntityDto> ProjectToList<TBasicEntityDto>(IQueryable<TEntity> query)
        {
            var selectMapTo = SelectMapToList<TBasicEntityDto>();
            return query.Select(selectMapTo);
        }

        protected virtual IQueryable<TDetailEntityDto> ProjectTo<TDetailEntityDto>(IQueryable<TEntity> query)
        {
            var selectMapTo = SelectMapTo<TDetailEntityDto>();
            return query.Select(selectMapTo);
        }

        protected virtual bool UserProjectTo { get; set; } = false;

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