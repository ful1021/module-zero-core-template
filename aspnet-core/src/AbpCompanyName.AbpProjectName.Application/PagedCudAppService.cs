using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using AbpCompanyName.AbpProjectName.Crud;

namespace AbpCompanyName.AbpProjectName
{
    /// <summary>
    /// 增删改 分页列表 父类
    /// </summary>
    public abstract class PagedCudAppService<TEntity, TBasicEntityDto, TDetailEntityDto, TPrimaryKey, TPagedListInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
: CudAppService<TEntity, TBasicEntityDto, TDetailEntityDto, TPrimaryKey, TPagedListInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
   where TEntity : class, IEntity<TPrimaryKey>
   where TBasicEntityDto : IEntityDto<TPrimaryKey>
   where TDetailEntityDto : IEntityDto<TPrimaryKey>
   where TPagedListInput : IPagedResultRequest, ILimitedResultRequest
   where TUpdateInput : IEntityDto<TPrimaryKey>
   where TGetInput : IEntityDto<TPrimaryKey>
   where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        protected PagedCudAppService(IRepository<TEntity, TPrimaryKey> repository) : base(repository)
        {
        }

        #region 分页列表

        /// <summary>
        /// Should apply paging if needed.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> query, TPagedListInput input)
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

        public virtual async Task<PagedResultDto<TBasicEntityDto>> PagedList(TPagedListInput input)
        {
            CheckPermission(PagedListPermissionName);

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

        #endregion 分页列表
    }

    public abstract class PagedCudAppService<TEntity, TBasicEntityDto, TDetailEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput>
    : PagedCudAppService<TEntity, TBasicEntityDto, TDetailEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>>
        where TEntity : class, IEntity<TPrimaryKey>
        where TBasicEntityDto : class, IEntityDto<TPrimaryKey>
        where TDetailEntityDto : class, IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ILimitedResultRequest
    {
        protected PagedCudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {
        }
    }
}