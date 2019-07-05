using System;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using AbpCompanyName.AbpProjectName.ExtendColumns.Dto;
using AbpCompanyName.AbpProjectName.Extensions;

namespace AbpCompanyName.AbpProjectName.ExtendColumns
{
    /// <summary>
    ///   服务实现
    /// </summary>
    public class ExtendColumnAppService : AsyncCrudAppServiceBase<ExtendColumn, ExtendColumnQueryDto, int, ExtendColumnGetAllInput, ExtendColumnCreateInput, ExtendColumnDto>, IExtendColumnAppService
    {
        private readonly IRepository<ExtendColumn, int> _extendColumnRepository;
        /// <summary>
        /// 构造函数
        /// </summary>        
        public ExtendColumnAppService(IRepository<ExtendColumn, int> extendColumnRepository) : base(extendColumnRepository)
        {
            _extendColumnRepository = extendColumnRepository;
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<ExtendColumn> CreateFilteredQuery(ExtendColumnGetAllInput input)
        {
            //var filters = input.Filter.ToStringArray();
            var creationTimeRange = input.CreationTime.ToDateTimeRange();
            var lastModificationTimeRange = input.LastModificationTime.ToDateTimeRange();
            
            return base.CreateFilteredQuery(input)
                //.WhereIf(filters.Any(), a => a.BizNo.Contains(input.Filter) || filters.Contains(a.BizNo))
                .WhereIf(creationTimeRange != null, d => d.CreationTime >= creationTimeRange.StartTime && d.CreationTime <= creationTimeRange.EndTime)
                
                .WhereIf(input.CreatorUserId.HasValue, a => a.CreatorUserId == input.CreatorUserId.Value)
                
                .WhereIf(input.Id?.Start != null, a => a.Id >= input.Id.Start.Value)
                .WhereIf(input.Id?.End != null, a => a.Id >= input.Id.End.Value)
                
                .WhereIf(!input.Key.IsNullOrWhiteSpace(), a => a.Key.Contains(input.Key))
                .WhereIf(lastModificationTimeRange != null, d => d.LastModificationTime >= lastModificationTimeRange.StartTime && d.LastModificationTime <= lastModificationTimeRange.EndTime)
                
                .WhereIf(input.LastModifierUserId.HasValue, a => a.LastModifierUserId == input.LastModifierUserId.Value)
                
                .WhereIf(input.TableName?.Start != null, a => a.TableName >= input.TableName.Start.Value)
                .WhereIf(input.TableName?.End != null, a => a.TableName >= input.TableName.End.Value)
                
                .WhereIf(!input.Title.IsNullOrWhiteSpace(), a => a.Title.Contains(input.Title))
                
                .WhereIf(input.Width?.Start != null, a => a.Width >= input.Width.Start.Value)
                .WhereIf(input.Width?.End != null, a => a.Width >= input.Width.End.Value)
                
                ;
        }
    }
}