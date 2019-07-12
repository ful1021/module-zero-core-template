using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AbpCompanyName.AbpProjectName.ExtendColumns.Dto;

namespace AbpCompanyName.AbpProjectName.ExtendColumns
{
    /// <summary>
    /// 扩展列  服务契约
    /// </summary>
    public interface IExtendColumnAppService : IApplicationService
    {
        Task<PagedResultDto<ExtendColumnQueryDto>> GetAll(ExtendColumnGetAllInput input);

        Task<ExtendColumnQueryDto> Get(EntityDto<int> input);

        Task<ExtendColumnQueryDto> Create(ExtendColumnCreateInput input);

        Task<ExtendColumnQueryDto> Update(ExtendColumnDto input);

        Task Delete(EntityDto<int> input);
    }
}