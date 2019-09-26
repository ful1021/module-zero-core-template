using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace AbpCompanyName.AbpProjectName.CustomColumns
{
    /// <summary>
    /// 自定义字段
    /// </summary>
    public class CustomColumn : AuditedAggregateRoot
    {
        public const int TitleMaxLength = 256;
        public const int OptionsMaxLength = 2048;

        /// <summary>
        /// 字段名称
        /// </summary>
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public CustomColumnType CustomColumnType { get; set; }

        /// <summary>
        /// 文本框值类型
        /// </summary>
        public CustomColumnValueType CustomColumnValueType { get; set; }

        /// <summary>
        /// 单选、多选、下拉等 选项 数组Json
        /// </summary>
        [MaxLength(OptionsMaxLength)]
        public string Options { get; set; }

        /// <summary>
        /// 如果是静态的，则不能修改并且不能删除
        /// </summary>
        public bool IsStatic { get; set; } = false;
    }
}