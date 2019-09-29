using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Abp.Application.Services.Dto;

namespace AbpCompanyName.AbpProjectName.Enumerations
{
    public class EnumNameValue : NameValueDto<int>
    {
        /// <summary>
        /// 文本说明.
        /// </summary>
        public string Label { get; set; }

        public int Order { get; set; }
    }

    public static class EnumNameValueExt
    {
        public static IEnumerable<EnumNameValue> GetEnum(this Type enumType)
        {
            var values = Enum.GetValues(enumType);

            var result = new List<EnumNameValue>();

            foreach (var item in values)
            {
                var fi = enumType.GetField(item.ToString());
                var attr = fi.GetCustomAttribute<DisplayAttribute>();
                var text = attr?.Name ?? fi?.Name ?? Enum.GetName(enumType, item);

                result.Add(new EnumNameValue
                {
                    Name = item.ToString(),
                    Label = text,
                    Value = Convert.ToInt32(item),
                    Order = attr?.GetOrder() ?? 0
                });
            }

            return result.OrderByDescending(r => r.Order).ToList();
        }
    }
}