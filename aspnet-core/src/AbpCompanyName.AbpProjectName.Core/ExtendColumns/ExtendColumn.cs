using Abp.Domain.Entities.Auditing;

namespace AbpCompanyName.AbpProjectName.ExtendColumns
{
    public class ExtendColumn : AuditedAggregateRoot
    {
        /// <summary>
        /// 支持扩展列的表
        /// </summary>
        public ExtendTableName TableName { get; set; }

        /// <summary>
        /// 对应列内容的字段名
        /// </summary>
        public string Key { get; set; }

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