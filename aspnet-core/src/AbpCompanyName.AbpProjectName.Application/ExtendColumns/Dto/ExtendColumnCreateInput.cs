using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.Runtime.Validation;

namespace AbpCompanyName.AbpProjectName.ExtendColumns.Dto
{
    /// <summary>
    /// 扩展列 新增输入参数
    /// </summary>
    [AutoMap(typeof(ExtendColumn))]
    public class ExtendColumnCreateInput : IShouldNormalize
    {
        /// <summary>
        /// 对应列内容的字段名
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 支持扩展列的表
        /// </summary>
        public AbpCompanyName.AbpProjectName.ExtendColumns.ExtendTableName TableName { get; set; }

        /// <summary>
        /// 列头显示文字
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 列宽
        /// </summary>
        public int Width { get; set; }

        
        public void Normalize()
        {
        }
    }    
}