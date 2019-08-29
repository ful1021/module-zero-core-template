using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using AbpCompanyName.AbpProjectName.Crud;

namespace AbpCompanyName.AbpProjectName
{
    /// <summary>
    /// 增删改 不分页列表 父类
    /// </summary>
    public abstract class NoPagedCudAppService<TEntity, TBasicEntityDto, TDetailEntityDto, TPrimaryKey, TListInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
: CudAppService<TEntity, TBasicEntityDto, TDetailEntityDto, TPrimaryKey, TListInput, TCreateInput, TUpdateInput, TGetInput, TDeleteInput>
   where TEntity : class, IEntity<TPrimaryKey>
   where TBasicEntityDto : IEntityDto<TPrimaryKey>
   where TDetailEntityDto : IEntityDto<TPrimaryKey>
   where TUpdateInput : IEntityDto<TPrimaryKey>
   where TGetInput : IEntityDto<TPrimaryKey>
   where TDeleteInput : IEntityDto<TPrimaryKey>
    {
        protected NoPagedCudAppService(IRepository<TEntity, TPrimaryKey> repository) : base(repository)
        {
        }

        #region 不分页列表

        /// <summary>
        /// 根据条件获取全部实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<ListResultDto<TBasicEntityDto>> List(TListInput input)
        {
            CheckPermission(ListPermissionName);

            var query = CreateFilteredQuery(input);
            query = ApplySorting(query, input);

            var entities = await ToListAsync(query);

            return new ListResultDto<TBasicEntityDto>(entities);
        }

        #endregion 不分页列表
    }

    public abstract class NoPagedCudAppService<TEntity, TBasicEntityDto, TDetailEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput>
    : NoPagedCudAppService<TEntity, TBasicEntityDto, TDetailEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput, EntityDto<TPrimaryKey>, EntityDto<TPrimaryKey>>
        where TEntity : class, IEntity<TPrimaryKey>
        where TBasicEntityDto : class, IEntityDto<TPrimaryKey>
        where TDetailEntityDto : class, IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
        where TGetAllInput : IPagedResultRequest, ILimitedResultRequest
    {
        protected NoPagedCudAppService(IRepository<TEntity, TPrimaryKey> repository)
            : base(repository)
        {
        }
    }
}