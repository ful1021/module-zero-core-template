using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq;
using AutoMapper.QueryableExtensions;

namespace AbpCompanyName.AbpProjectName
{
    public abstract class AsyncCrudAppServiceBase<TEntity, TBasicEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
        : AsyncCrudAppService<TEntity, TBasicEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>,
        IAsyncCrudAppService<TBasicEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
           where TEntity : class, IEntity<TPrimaryKey>
           where TBasicEntityDto : IEntityDto<TPrimaryKey>
           where TUpdateInput : IEntityDto<TPrimaryKey>
           where TGetInput : IEntityDto<TPrimaryKey>
           where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        protected virtual string GetAllListPermissionName { get; set; }

        /// <summary>
        /// 父类构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        protected AsyncCrudAppServiceBase(IRepository<TEntity, TPrimaryKey> repository) : base(repository)
        {
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        #region 查询

        public override async Task<PagedResultDto<TBasicEntityDto>> GetAll(TGetAllInput input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);
            var noPagerQuery = query;
            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await ToListAsync(query);

            int totalCount = entities?.Count ?? 0;
            var pagedInput = input as IPagedResultRequest;
            //没有分页参数,不需要统计总记录数  或者 总条数 小于 每页大小，则代表只有1页数据
            if (pagedInput != null || totalCount < pagedInput.MaxResultCount)
            {
                totalCount = await AsyncQueryableExecuter.CountAsync(noPagerQuery);
            }

            return new PagedResultDto<TBasicEntityDto>(totalCount, entities);
        }

        /// <summary>
        /// 根据条件获取全部实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ListResultDto<TBasicEntityDto>> GetAllList(TGetAllInput input)
        {
            CheckPermission(GetAllListPermissionName);

            var query = CreateFilteredQuery(input);
            query = ApplySorting(query, input);

            var entities = await ToListAsync(query);

            return new ListResultDto<TBasicEntityDto>(entities);
        }

        public override Task<TBasicEntityDto> Get(TGetInput input)
        {
            CheckGetPermission();

            return FirstOrDefaultAsync(input);
        }

        #endregion 查询

        #region ProjectTo

        /// <summary>
        /// 将实体模型中的每个元素映射到 TEntityQueryDto
        /// </summary>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, TBasicEntityDto>> SelectMapTo()
        {
            return null;
        }

        private IQueryable<TBasicEntityDto> ProjectTo(IQueryable<TEntity> query)
        {
            var selectMapTo = SelectMapTo();
            if (selectMapTo == null)
            {
                return query.ProjectTo<TBasicEntityDto>();
            }
            return query.Select(selectMapTo);
        }

        protected virtual bool UserProjectTo { get; set; } = false;

        protected virtual async Task<TBasicEntityDto> FirstOrDefaultAsync(TGetInput input)
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

        protected virtual async Task<List<TBasicEntityDto>> ToListAsync(IQueryable<TEntity> query)
        {
            if (UserProjectTo)
            {
                return await AsyncQueryableExecuter.ToListAsync(ProjectTo(query));
            }
            var entities = await AsyncQueryableExecuter.ToListAsync(query);
            return entities.Select(MapToEntityDto).ToList();
        }

        #endregion ProjectTo
    }

    public abstract class AsyncCrudAppServiceBase<TEntity, TEntityQueryDto>
     : AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, int>
     where TEntity : class, IEntity<int>
     where TEntityQueryDto : class, IEntityDto<int>
    {
        protected AsyncCrudAppServiceBase(IRepository<TEntity, int> repository)
            : base(repository)
        {
        }
    }

    public abstract class AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey>
        : AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey, PagedAndSortedResultRequestDto>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityQueryDto : class, IEntityDto<TPrimaryKey>
    {
        protected AsyncCrudAppServiceBase(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {
        }
    }

    public abstract class AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey, TGetAllInput>
        : AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey, TGetAllInput, TEntityQueryDto, TEntityQueryDto>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityQueryDto : class, IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ILimitedResultRequest
    {
        protected AsyncCrudAppServiceBase(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {
        }
    }

    public abstract class AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey, TGetAllInput, TCreateInput>
        : AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey, TGetAllInput, TCreateInput, TCreateInput>
        where TGetAllInput : IPagedResultRequest, ILimitedResultRequest
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityQueryDto : class, IEntityDto<TPrimaryKey>
        where TCreateInput : IEntityDto<TPrimaryKey>
    {
        protected AsyncCrudAppServiceBase(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {
        }
    }

    public abstract class AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput>
    : AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, EntityDto<TPrimaryKey>>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityQueryDto : class, IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ILimitedResultRequest
    {
        protected AsyncCrudAppServiceBase(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {
        }
    }

    public abstract class AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput>
    : AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, EntityDto<TPrimaryKey>>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityQueryDto : class, IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ILimitedResultRequest
    {
        protected AsyncCrudAppServiceBase(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {
        }
    }
}