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
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public int Level { get; set; }

        public int? ParentId { get; set; }

        public ICollection<DataDictionaryQueryDto> Children { get; set; }
    }
}