using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Abp.Authorization;
using Abp.Extensions;

namespace AbpCompanyName.AbpProjectName.Enumerations
{
    public class EnumsAppService : AbpProjectNameAppServiceBase, IEnumsAppService
    {
        private readonly IEnumDataConventionalRegistrar _enumDataRegistrar;

        public EnumsAppService(IEnumDataConventionalRegistrar enumDataRegistrar)
        {
            _enumDataRegistrar = enumDataRegistrar;
        }

        /// <summary>
        /// 批量获取 跟据枚举名称获取可用值列表
        /// </summary>
        /// <param name="regKeys">注册的枚举名称，多个以,分割</param>
        /// <returns></returns>
        public Dictionary<string, IEnumerable<EnumNameValue>> BatchGetEnumList(string regKeys)
        {
            if (regKeys == null) throw new ArgumentNullException(nameof(regKeys));

            var keyList = regKeys.Split(',');

            var types = _enumDataRegistrar
                .GetRegisteredEmumDataList()
                .Where(t => keyList.Contains(t.Name))
                .ToList();

            var result = types.ToDictionary(t => t.Name.ToCamelCase(), t => t.GetEnum());

            return result;
        }
    }
}