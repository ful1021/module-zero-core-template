using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Dependency;

namespace AbpCompanyName.AbpProjectName.Enumerations
{
    public class EnumDataConventionalRegistrar : IEnumDataConventionalRegistrar, ISingletonDependency
    {
        private static readonly List<Type> EnumDataList = new List<Type>();

        public void AssemblyConventionalRegister(params Assembly[] assembly)
        {
            if (assembly == null || assembly.Length == 0) throw new ArgumentNullException(nameof(assembly));

            foreach (var item in assembly)
            {
                var enums = item.GetTypes().Where(t => t.IsEnum && t.IsPublic && (t.GetCustomAttribute<EnumDataAttribute>()?.IsEnabled ?? false)).ToList();

                foreach (var @enum in enums)
                {
                    EnumDataList.AddIfNotContains(@enum);
                }
            }


        }

        public IEnumerable<Type> GetRegisteredEmumDataList()
        {
            return EnumDataList;
        }
    }
}