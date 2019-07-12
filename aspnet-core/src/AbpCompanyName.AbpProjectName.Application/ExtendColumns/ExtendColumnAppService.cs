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
    /// 扩展列  服务实现
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
            var titles = input.Title.ToStringArray();

            return base.CreateFilteredQuery(input)
                //.WhereIf(filters.Any(), a => a.BizNo.Contains(input.Filter) || filters.Contains(a.BizNo))
                .WhereIf(!input.Key.IsNullOrWhiteSpace(), a => a.Key.Contains(input.Key))

                //.WhereIf(input.TableName?.Start != null, a => a.TableName >= input.TableName.Start.Value)
                //.WhereIf(input.TableName?.End != null, a => a.TableName >= input.TableName.End.Value)

                .WhereIf(titles.Any(), a => a.Title.Contains(input.Title) || titles.Contains(a.Title))

                //.WhereIf(input.Width?.Start != null, a => a.Width >= input.Width.Start.Value)
                //.WhereIf(input.Width?.End != null, a => a.Width >= input.Width.End.Value)

                ;
        }
    }
}