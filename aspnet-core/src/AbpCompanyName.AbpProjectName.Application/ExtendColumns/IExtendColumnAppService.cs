using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AbpCompanyName.AbpProjectName.ExtendColumns.Dto;

namespace AbpCompanyName.AbpProjectName.ExtendColumns
{
    public interface IExtendColumnAppService : IApplicationService
    {
        Task<PagedResultDto<ExtendColumnQueryDto>> GetAll(PagedAndSortedResultRequestDto input);

        Task Delete(EntityDto<int> input);
    }
}