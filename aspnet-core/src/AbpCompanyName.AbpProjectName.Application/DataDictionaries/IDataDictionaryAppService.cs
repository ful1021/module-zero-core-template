using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AbpCompanyName.AbpProjectName.DataDictionaries.Dto;

namespace AbpCompanyName.AbpProjectName.DataDictionaries
{
    public interface IDataDictionaryAppService : IApplicationService
    {
        Task<PagedResultDto<DataDictionaryQueryDto>> GetAll(PagedAndSortedResultRequestDto input);

        Task Delete(EntityDto<int> input);
    }
}