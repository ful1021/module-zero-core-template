using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace AbpCompanyName.AbpProjectName
{
    /// <summary>
    /// Crud  服务实现
    /// </summary>
    public abstract class CrudAppService<TEntity, TBasicEntityDto, TDetailEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput> : AppServiceBase<TEntity, TPrimaryKey>
  where TEntity : class, IEntity<TPrimaryKey>
  where TBasicEntityDto : IEntityDto<TPrimaryKey>
  where TDetailEntityDto : IEntityDto<TPrimaryKey>
  where TGetAllInput : PagedAndSortedResultRequestDto
  where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CrudAppService(IRepository<TEntity, TPrimaryKey> repository) : base(repository)
        {
        }

        protected virtual IQueryable<TEntity> CreateFilteredQuery(TGetAllInput input)
        {
            return Repository.GetAll();
        }

        public async Task<PagedResultDto<TBasicEntityDto>> PagedList(TGetAllInput input)
        {
            CheckGetAllPermission();
            var query = CreateFilteredQuery(input);
            return await base.ToPagedList<TGetAllInput, TBasicEntityDto>(query, input);
        }

        public virtual async Task<TDetailEntityDto> Get(EntityDto<TPrimaryKey> input)
        {
            CheckGetPermission();
            var entity = await GetEntityByIdAsync(input.Id);
            return MapToEntityDto<TDetailEntityDto>(entity);
        }

        #region 增删改

        public virtual async Task<TDetailEntityDto> Create(TCreateInput input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto<TDetailEntityDto>(entity);
        }

        public virtual async Task<TDetailEntityDto> Update(TUpdateInput input)
        {
            CheckUpdatePermission();

            var entity = await GetEntityByIdAsync(input.Id);

            MapToEntity(input, entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto<TDetailEntityDto>(entity);
        }

        public virtual Task Delete(EntityDto<TPrimaryKey> input)
        {
            CheckDeletePermission();

            return Repository.DeleteAsync(input.Id);
        }

        protected virtual Task<TEntity> GetEntityByIdAsync(TPrimaryKey id)
        {
            return Repository.GetAsync(id);
        }

        #endregion 增删改

        protected virtual string GetPermissionName { get; set; }

        protected virtual string GetAllPermissionName { get; set; }

        protected virtual string CreatePermissionName { get; set; }

        protected virtual string UpdatePermissionName { get; set; }

        protected virtual string DeletePermissionName { get; set; }

        protected virtual void CheckGetPermission()
        {
            CheckPermission(GetPermissionName);
        }

        protected virtual void CheckGetAllPermission()
        {
            CheckPermission(GetAllPermissionName);
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
    }
}