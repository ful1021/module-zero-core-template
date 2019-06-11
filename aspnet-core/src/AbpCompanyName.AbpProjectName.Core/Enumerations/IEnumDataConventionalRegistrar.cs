using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace AbpCompanyName.AbpProjectName.Enumerations
{
    public interface IEnumDataConventionalRegistrar
    {
        /// <summary>
        /// 注册所有标记<see cref="EnumDataAttribute"/>的枚举
        /// </summary>
        /// <param name="assembly"></param>
        void AssemblyConventionalRegister([NotNull]params Assembly[] assembly);

        /// <summary>
        /// 获取已注册的枚举数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetRegisteredEmumDataList();
    }
}