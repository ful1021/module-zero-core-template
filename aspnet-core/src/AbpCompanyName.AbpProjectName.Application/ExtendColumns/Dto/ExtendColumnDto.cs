using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace AbpCompanyName.AbpProjectName.ExtendColumns.Dto
{
    /// <summary>
    /// 扩展列 新增输入参数
    /// </summary>
    [AutoMapTo(typeof(ExtendColumn))]
    public class ExtendColumnDto : EntityDto<int>
    {
        /// <summary>
        /// 对应列内容的字段名
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 支持扩展列的表
        /// </summary>
        public ExtendTableName TableName { get; set; }

        /// <summary>
        /// 列头显示文字
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 列宽
        /// </summary>
        public int Width { get; set; }
    }
}