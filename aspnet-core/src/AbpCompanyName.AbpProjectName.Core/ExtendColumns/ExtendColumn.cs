using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace AbpCompanyName.AbpProjectName.ExtendColumns
{
    /// <summary>
    /// 扩展列
    /// </summary>
    public class ExtendColumn : AuditedAggregateRoot
    {
        public const int KeyMaxLength = 128;
        public const int TitleMaxLength = 256;

        /// <summary>
        /// 支持扩展列的表
        /// </summary>
        public ExtendTableName TableName { get; set; }

        /// <summary>
        /// 对应列内容的字段名
        /// </summary>
        [MaxLength(KeyMaxLength)]
        public string Key { get; set; }

        /// <summary>
        /// 列头显示文字
        /// </summary>
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; }

        /// <summary>
        /// 列宽
        /// </summary>
        public int Width { get; set; }
    }
}