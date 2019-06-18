using Abp.Domain.Repositories;
using AbpCompanyName.AbpProjectName.ExtendColumns.Dto;

namespace AbpCompanyName.AbpProjectName.ExtendColumns
{
    public class ExtendColumnAppService : AsyncCrudAppServiceBase<ExtendColumn, ExtendColumnQueryDto>, IExtendColumnAppService
    {
        public ExtendColumnAppService(IRepository<ExtendColumn> repository) : base(repository)
        {
        }
    }
}