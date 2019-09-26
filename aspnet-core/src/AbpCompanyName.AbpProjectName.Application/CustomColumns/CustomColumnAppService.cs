using Abp.Domain.Repositories;

namespace AbpCompanyName.AbpProjectName.CustomColumns
{
    /// <summary>
    /// 自定义字段  服务实现
    /// </summary>
    public class CustomColumnAppService : AppServiceBase<CustomColumn, int>, ICustomColumnAppService
    {
        private readonly IRepository<CustomColumn> _bizLogRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomColumnAppService(IRepository<CustomColumn> bizLogRepository) : base(bizLogRepository)
        {
            _bizLogRepository = bizLogRepository;
        }
    }
}