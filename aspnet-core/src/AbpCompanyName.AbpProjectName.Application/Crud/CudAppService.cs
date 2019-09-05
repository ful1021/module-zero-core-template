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
using AutoMapper.QueryableExtensions;

namespace AbpCompanyName.AbpProjectName.Crud
{
    /// <summary>
    /// 增删改 父类
    /// </summary>
    public abstract class CudAppService<TEntity, TBasicEntityDto, TDetailEntityDto, TPrimaryKey, TListInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
: ApplicationService
  where TEntity : class, IEntity<TPrimaryKey>
  where TBasicEntityDto : IEntityDto<TPrimaryKey>
  where TDetailEntityDto : IEntityDto<TPrimaryKey>
  where TUpdateInput : IEntityDto<TPrimaryKey>
  where TGetInput : IEntityDto<TPrimaryKey>
  where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        protected virtual string GetPermissionName { get; set; }
        protected virtual string ListPermissionName { get; set; }
        protected virtual string PagedListPermissionName { get; set; }
        protected virtual string CreatePermissionName { get; set; }
        protected virtual string UpdatePermissionName { get; set; }
        protected virtual string DeletePermissionName { get; set; }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        protected readonly IRepository<TEntity, TPrimaryKey> Repository;

        /// <summary>
        /// 父类构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        protected CudAppService(IRepository<TEntity, TPrimaryKey> repository)
        {
            Repository = repository;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        #region 查询

        /// <summary>
        /// Should apply sorting if needed.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, TListInput input)
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

        /// <summary>
        /// This method should create <see cref="IQueryable{TEntity}"/> based on given input.
        /// It should filter query if needed, but should not do sorting or paging.
        /// </summary>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TEntity> CreateFilteredQuery(TListInput input)
        {
            return Repository.GetAll();
        }

        protected virtual async Task<List<TBasicEntityDto>> ToListAsync(IQueryable<TEntity> query)
        {
            if (UserProjectTo)
            {
                return await AsyncQueryableExecuter.ToListAsync(ProjectToList(query));
            }
            var entities = await AsyncQueryableExecuter.ToListAsync(query);

            return entities.Select(MapToList).ToList();
        }

        #endregion 查询

        #region 查询一条记录

        public virtual async Task<TDetailEntityDto> Get(TGetInput input)
        {
            CheckGetPermission();

            return await FirstOrDefaultAsync(input);
        }

        protected virtual Task<TEntity> GetEntityByIdAsync(TPrimaryKey id)
        {
            return Repository.GetAsync(id);
        }

        protected virtual async Task<TDetailEntityDto> FirstOrDefaultAsync(TGetInput input)
        {
            if (UserProjectTo)
            {
                var query = Repository.GetAll().Where(CreateEqualityExpressionForId(input.Id));
                return await AsyncQueryableExecuter.FirstOrDefaultAsync(ProjectTo(query));
            }
            var info = await Repository.FirstOrDefaultAsync(input.Id);
            return MapToEntityDto(info);
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

        #region 增删改

        public virtual async Task<TDetailEntityDto> Create(TCreateInput input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public virtual async Task<TDetailEntityDto> Update(TUpdateInput input)
        {
            CheckUpdatePermission();

            var entity = await GetEntityByIdAsync(input.Id);

            MapToEntity(input, entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public virtual Task Delete(TDeleteInput input)
        {
            CheckDeletePermission();

            return Repository.DeleteAsync(input.Id);
        }

        #endregion 增删改

        #region MapTo

        protected virtual TDetailEntityDto MapToEntityDto(TEntity entity)
        {
            return ObjectMapper.Map<TDetailEntityDto>(entity);
        }

        protected virtual TEntity MapToEntity(TCreateInput createInput)
        {
            return ObjectMapper.Map<TEntity>(createInput);
        }

        protected virtual void MapToEntity(TUpdateInput updateInput, TEntity entity)
        {
            ObjectMapper.Map(updateInput, entity);
        }

        /// <summary>
        /// 将实体模型中的每个元素映射到 TEntityQueryDto
        /// </summary>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, TBasicEntityDto>> SelectMapToList()
        {
            return null;
        }

        /// <summary>
        /// 将实体模型中的每个元素映射到 TEntityQueryDto
        /// </summary>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, TDetailEntityDto>> SelectMapTo()
        {
            return null;
        }

        private IQueryable<TBasicEntityDto> ProjectToList(IQueryable<TEntity> query)
        {
            var selectMapTo = SelectMapToList();
            if (selectMapTo == null)
            {
                return query.ProjectTo<TBasicEntityDto>();
            }
            return query.Select(selectMapTo);
        }

        private IQueryable<TDetailEntityDto> ProjectTo(IQueryable<TEntity> query)
        {
            var selectMapTo = SelectMapTo();
            if (selectMapTo == null)
            {
                return query.ProjectTo<TDetailEntityDto>();
            }
            return query.Select(selectMapTo);
        }

        protected virtual TBasicEntityDto MapToList(TEntity entity)
        {
            return ObjectMapper.Map<TBasicEntityDto>(entity);
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

        protected virtual void CheckGetPermission()
        {
            CheckPermission(GetPermissionName);
        }

        protected virtual void CheckCreatePermission()
        {
            CheckPermission(CreatePermissionName);
        }

        protected virtual void CheckUpdatePermission()
        {
            CheckPermission(UpdatePermissionName);
        }

        protected virtual void CheckDeletePermission()
        {
            CheckPermission(DeletePermissionName);
        }

        #endregion 检查权限
    }
}