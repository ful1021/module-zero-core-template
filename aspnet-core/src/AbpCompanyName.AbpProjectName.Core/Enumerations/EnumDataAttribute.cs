using System;

namespace AbpCompanyName.AbpProjectName.Enumerations
{
    /// <summary>
    /// 枚举数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = true)]
    public class EnumDataAttribute : Attribute
    {
        /// <summary>
        /// 是否映射枚举数据，Default: true.
        /// </summary>
        public bool IsEnabled { get; set; } = true;


    }

}
