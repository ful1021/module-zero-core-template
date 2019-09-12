using System;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using AbpCompanyName.AbpProjectName.BizLogs.Dto;
using AbpCompanyName.AbpProjectName.Extensions;

namespace AbpCompanyName.AbpProjectName.BizLogs
{
    /// <summary>
    /// 业务日志  服务实现
    /// </summary>
    public class BizLogAppService : AppServiceBase<BizLog, Guid>, IBizLogAppService
    {
        private readonly IRepository<BizLog, Guid> _bizLogRepository;
        /// <summary>
        /// 构造函数
        /// </summary>        
        public BizLogAppService(IRepository<BizLog, Guid> bizLogRepository) : base(bizLogRepository)
        {
            _bizLogRepository = bizLogRepository;
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected IQueryable<BizLog> CreateFilteredQuery(BizLogGetAllInput input)
        {
            //var filters = input.Filter.ToStringArray();
            var creationTimeRange = input.CreationTime.ToDateTimeRange();

            return Repository.GetAll()
                //.WhereIf(filters.Any(), a => a.BizNo.Contains(input.Filter) || filters.Contains(a.BizNo))
                .WhereIf(!input.BizData.IsNullOrWhiteSpace(), a => a.BizData.Contains(input.BizData))
                .WhereIf(!input.BizDescription.IsNullOrWhiteSpace(), a => a.BizDescription.Contains(input.BizDescription))
                .WhereIf(!input.BizName.IsNullOrWhiteSpace(), a => a.BizName.Contains(input.BizName))
                .WhereIf(!input.BizNo.IsNullOrWhiteSpace(), a => a.BizNo.Contains(input.BizNo))
                .WhereIf(!input.BizType.IsNullOrWhiteSpace(), a => a.BizType.Contains(input.BizType))
                .WhereIf(creationTimeRange != null, d => d.CreationTime >= creationTimeRange.StartTime && d.CreationTime <= creationTimeRange.EndTime)

                .WhereIf(input.CreatorUserId.HasValue, a => a.CreatorUserId == input.CreatorUserId.Value)
                .WhereIf(!input.ExtensionData.IsNullOrWhiteSpace(), a => a.ExtensionData.Contains(input.ExtensionData))
                .WhereIf(input.Id.HasValue, a => a.Id == input.Id.Value)
                ;
        }
    }
}