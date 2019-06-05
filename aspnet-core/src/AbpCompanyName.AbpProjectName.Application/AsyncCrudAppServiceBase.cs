using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;

namespace AbpCompanyName.AbpProjectName
{
    public abstract class AsyncCrudAppServiceBase<TEntity, TEntityQueryDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
        : AsyncCrudAppService<TEntity, TEntityQueryDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityQueryDto : class, IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetInput : IEntityDto<TPrimaryKey>
        where TDeleteInput : IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ILimitedResultRequest
    {
        /// <summary>
        /// 父类构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        protected AsyncCrudAppServiceBase(IRepository<TEntity, TPrimaryKey> repository) : base(repository)
        {
        }

        public override Task<TEntityQueryDto> Get(TGetInput input)
        {
            CheckGetPermission();

            var query = Repository.GetAll().Where(CreateEqualityExpressionForId(input.Id));
            return AsyncQueryableExecuter.FirstOrDefaultAsync(ProjectTo(query));
        }

        protected Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        public override async Task<PagedResultDto<TEntityQueryDto>> GetAll(TGetAllInput input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);
            var noPagerQuery = query;
            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(ProjectTo(query));

            int totalCount = entities?.Count ?? 0;
            var pagedInput = input as IPagedResultRequest;
            //没有分页参数,不需要统计总记录数
            if (pagedInput != null)
            {
                totalCount = await AsyncQueryableExecuter.CountAsync(noPagerQuery);
            }

            return new PagedResultDto<TEntityQueryDto>(totalCount, entities);
        }

        /// <summary>
        /// 根据条件获取全部实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ListResultDto<TEntityQueryDto>> GetAllList(TGetAllInput input)
        {
            var query = CreateFilteredQuery(input);
            query = ApplySorting(query, input);

            var list = await AsyncQueryableExecuter.ToListAsync(ProjectTo(query));

            return new ListResultDto<TEntityQueryDto>(list);
        }

        /// <summary>
        /// 将实体模型中的每个元素映射到 TEntityQueryDto
        /// </summary>
        /// <returns></returns>
        protected virtual Expression<Func<TEntity, TEntityQueryDto>> SelectMapTo()
        {
            return null;
        }

        private IQueryable<TEntityQueryDto> ProjectTo(IQueryable<TEntity> query)
        {
            var selectMapTo = SelectMapTo();
            if (selectMapTo == null)
            {
                return query.ProjectTo<TEntityQueryDto>();
            }
            return query.Select(selectMapTo);
        }
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