using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AbpCompanyName.AbpProjectName.ExtendColumns.Dto;

namespace AbpCompanyName.AbpProjectName.ExtendColumns
{
    public interface IExtendColumnAppService : IApplicationService
    {
        Task<PagedResultDto<ExtendColumnQueryDto>> GetAll(ExtendColumnGetAllInput input);

        Task<ExtendColumnQueryDto> Create(ExtendColumnDto input);

        Task<ExtendColumnQueryDto> Update(ExtendColumnDto input);

        Task Delete(EntityDto<int> input);
    }
}