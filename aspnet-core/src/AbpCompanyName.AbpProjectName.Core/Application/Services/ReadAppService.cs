using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace Boss.Hr
{
    /// <summary>
    /// Crud  服务实现
    /// </summary>
    public abstract class ReadAppService<TEntity, TBasicEntityDto, TPrimaryKey, TGetAllInput> : AppServiceBase<TEntity, TPrimaryKey>
  where TEntity : class, IEntity<TPrimaryKey>
  where TBasicEntityDto : IEntityDto<TPrimaryKey>
  where TGetAllInput : IPagedAndSortedResultRequest
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ReadAppService(IRepository<TEntity, TPrimaryKey> repository) : base(repository)
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
            return await base.ToPagedList<TBasicEntityDto>(query, input);
        }

        protected virtual string GetAllPermissionName { get; set; }

        protected virtual void CheckGetAllPermission()
        {
            CheckPermission(GetAllPermissionName);
        }
    }
}