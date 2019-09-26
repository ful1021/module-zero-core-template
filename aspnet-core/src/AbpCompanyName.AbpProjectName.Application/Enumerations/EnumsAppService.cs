using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Abp.Authorization;
using Abp.Extensions;
using AbpCompanyName.AbpProjectName.Enumerations.Dto;

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
        public Dictionary<string, IEnumerable<EnumNameValueDto>> BatchGetEnumList(string regKeys)
        {
            if (regKeys == null) throw new ArgumentNullException(nameof(regKeys));

            var keyList = regKeys.Split(',');

            var types = _enumDataRegistrar
                .GetRegisteredEmumDataList()
                .Where(t => keyList.Contains(t.Name))
                .ToList();

            var result = types.ToDictionary(t => t.Name.ToCamelCase(), GetEnumMembers);

            return result;
        }

        private IEnumerable<EnumNameValueDto> GetEnumMembers(Type enumType)
        {
            var values = Enum.GetValues(enumType);

            var result = new List<EnumNameValueDto>();

            foreach (var item in values)
            {
                var fi = enumType.GetField(item.ToString());
                var attr = fi.GetCustomAttribute<DisplayAttribute>();
                var text = attr?.Name ?? fi?.Name ?? Enum.GetName(enumType, item);

                result.Add(new EnumNameValueDto
                {
                    Name = item.ToString(),
                    Text = text,
                    Value = Convert.ToInt32(item),
                    Order = attr?.GetOrder() ?? 0
                });
            }

            return result.OrderByDescending(r => r.Order).ToList();
        }
    }
}