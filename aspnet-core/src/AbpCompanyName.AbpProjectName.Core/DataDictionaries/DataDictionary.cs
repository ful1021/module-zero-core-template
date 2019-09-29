using System.ComponentModel.DataAnnotations;
using Abp.GeneralTree;

namespace AbpCompanyName.AbpProjectName.DataDictionaries
{
    public class DataDictionary : TreeEntity<DataDictionary, int>
    {
        public const int TypeCodeMaxLength = 128;
        public const int TypeNameMaxLength = 256;

        /// <summary>
        /// 类型编码
        /// </summary>
        [MaxLength(TypeCodeMaxLength)]
        public virtual string TypeCode { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        [MaxLength(TypeNameMaxLength)]
        public virtual string TypeName { get; set; }
    }
}