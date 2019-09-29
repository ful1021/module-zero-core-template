using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.GeneralTree;

namespace AbpCompanyName.AbpProjectName.DataDictionaries.Dto
{
    [AutoMap(typeof(DataDictionary))]
    public class DataDictionaryQueryDto : EntityDto, IGeneralTreeDto<DataDictionaryQueryDto, int>
    {
        /// <summary>
        /// 类型编码
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public int Level { get; set; }

        /// <summary>
        /// 排序 ，越小则越向前
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 是否允许添加子节点
        /// </summary>
        public bool IsAllowAddChildren { get; set; }

        /// <summary>
        /// 如果为 true，则不能修改或者删除
        /// </summary>
        public bool IsStatic { get; set; }

        public int? ParentId { get; set; }

        public ICollection<DataDictionaryQueryDto> Children { get; set; }
    }
}