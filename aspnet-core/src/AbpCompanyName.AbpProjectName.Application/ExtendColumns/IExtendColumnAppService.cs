using System;
using Abp.Application.Services;
using AbpCompanyName.AbpProjectName.ExtendColumns.Dto;

namespace AbpCompanyName.AbpProjectName.ExtendColumns
{
    /// <summary>
    ///   服务契约
    /// </summary>
    public interface IExtendColumnAppService : IAsyncCrudAppService<ExtendColumnQueryDto, int, ExtendColumnGetAllInput, ExtendColumnCreateInput, ExtendColumnDto>
    {
    }
}
