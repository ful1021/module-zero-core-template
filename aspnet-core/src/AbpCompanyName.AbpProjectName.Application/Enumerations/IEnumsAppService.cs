using System.Collections.Generic;
using Abp.Application.Services;
using AbpCompanyName.AbpProjectName.Enumerations.Dto;

namespace AbpCompanyName.AbpProjectName.Enumerations
{
    public interface IEnumsAppService : IApplicationService
    {
        /// <summary>
        /// 批量获取 跟据枚举名称获取可用值列表
        /// </summary>
        /// <param name="regKeys">注册的枚举名称，多个以,分割</param>
        /// <returns></returns>
        Dictionary<string, IEnumerable<EnumNameValueDto>> BatchGetEnumList(string regKeys);
    }
}