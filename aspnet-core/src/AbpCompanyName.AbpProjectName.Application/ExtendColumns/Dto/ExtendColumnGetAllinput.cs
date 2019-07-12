using Abp.Application.Services.Dto;

namespace AbpCompanyName.AbpProjectName.ExtendColumns.Dto
{
    /// <summary>
    /// 扩展列   获取所有 输入参数
    /// </summary>
    public class ExtendColumnGetAllInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExtendColumnGetAllInput()
        {
            Sorting = "CreationTime DESC";
        }

        /// <summary>
        /// 对应列内容的字段名
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 支持扩展列的表
        /// </summary>
        public AbpCompanyName.AbpProjectName.ExtendColumns.ExtendTableName? TableName { get; set; }

        /// <summary>
        /// 列头显示文字
        /// </summary>
        public string Title { get; set; }
    }
}