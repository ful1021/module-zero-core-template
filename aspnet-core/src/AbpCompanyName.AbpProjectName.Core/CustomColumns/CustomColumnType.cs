using System.ComponentModel.DataAnnotations;
using AbpCompanyName.AbpProjectName.Enumerations;

namespace AbpCompanyName.AbpProjectName.CustomColumns
{
    [EnumData]
    public enum CustomColumnType
    {
        /// <summary>
        /// 文本框
        /// </summary>
        [Display(Name = "文本框")]
        TextBox = 0,

        /// <summary>
        /// 多选框
        /// </summary>
        [Display(Name = "多选框")]
        CheckBox = 1,

        /// <summary>
        /// 单选按钮
        /// </summary>
        [Display(Name = "单选按钮")]
        RadioButton = 2,

        /// <summary>
        /// 下拉列表
        /// </summary>
        [Display(Name = "下拉列表")]
        DropdownList = 3,

        ///// <summary>
        ///// 可编辑的下拉列表
        ///// </summary>
        //EditableDropdownList = 4
    }
}