using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AbpCompanyName.AbpProjectName.Domain.Entities;

namespace AbpCompanyName.AbpProjectName.DataDictionaries.Dto
{
    [AutoMap(typeof(DataDictionary))]
    public class DataDictionaryDto : EntityDto
    {
        /// <summary>
        /// 类型编码
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public string FullName { get; set; }

        /// <summary>
        /// 名称值类型
        /// </summary>
        public StringTextType NameTextType { get; set; }

        /// <summary>
        /// 排序【越小越靠前】
        /// </summary>
        public int Sort { get; set; } = 10000;

        /// <summary>
        /// A JSON formatted string to extend the containing object.
        /// </summary>
        public string ExtensionData { get; set; }

        public int Level { get; set; }
    }
}