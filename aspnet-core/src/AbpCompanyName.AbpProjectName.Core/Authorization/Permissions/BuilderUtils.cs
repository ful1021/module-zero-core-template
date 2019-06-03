using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;
using Newtonsoft.Json;

namespace AbpCompanyName.AbpProjectName.Authorization.Permissions
{
    public static class BuilderUtils
    {
        public const string RootNamespace = "AbpCompanyName.AbpProjectName.Authorization.Permissions";

        public static void Build(this IPermissionDefinitionContext context, string name)
        {
            var list = GetPermissionJson(name);
            foreach (var item in list)
            {
                var module = context.CreatePermission(
                    item.Name,
                    new FixedLocalizableString(item.DisplayName),
                    multiTenancySides: GetMultiTenancySides(item));

                var children = item.GetChildren();
                BuildChildrenPermission(children, module, item.Name);
            }
        }

        public static List<PermissionJson> GetPermissionJson(string name)
        {
            var asm = Assembly.GetExecutingAssembly();//读取嵌入式资源
            var sm = asm.GetManifestResourceStream(RootNamespace + "." + name + ".json");
            var list = GetPermissionJson(sm);
            return list;
        }

        private static List<PermissionJson> GetPermissionJson(Stream sm)
        {
            using (var reader = new StreamReader(sm))
            {
                string text = reader.ReadToEnd();
                var json = JsonConvert.DeserializeObject<List<PermissionJson>>(text).OrderBy(c => c.Order).ToList();
                return json;
            }
        }

        private static void BuildChildrenPermission(List<PermissionJson> children, Permission module, string parentName)
        {
            foreach (var item in children)
            {
                var name = $"{parentName}.{item.Name}";
                var itemModule = module.CreateChildPermission(name,
                    new FixedLocalizableString(item.DisplayName),
                    multiTenancySides: GetMultiTenancySides(item));
                var itemchildren = item.GetChildren();
                if (itemchildren.Any())
                {
                    BuildChildrenPermission(itemchildren, itemModule, name);
                }
            }
        }

        public static MultiTenancySides GetMultiTenancySides(PermissionJson item)
        {
            if (item.MultiTenancySide.HasValue)
            {
                return (MultiTenancySides)item.MultiTenancySide.Value;
            }
            return MultiTenancySides.Tenant | MultiTenancySides.Host;
        }
    }
}