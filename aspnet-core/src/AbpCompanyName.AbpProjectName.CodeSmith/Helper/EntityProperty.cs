using System.Reflection;

namespace AbpCompanyName.AbpProjectName.Helper
{
    public class EntityProperty
    {
        /// <summary>
        /// 实体 所有 属性
        /// </summary>
        public PropertyInfo[] EntityProps { get; set; }

        /// <summary>
        /// 实体 时间类型 属性
        /// </summary>
        public PropertyInfo[] EntityDateTimeProps { get; set; }

        /// <summary>
        /// 实体 枚举类型 属性
        /// </summary>
        public PropertyInfo[] EntityEnumsProps { get; set; }
    }
}