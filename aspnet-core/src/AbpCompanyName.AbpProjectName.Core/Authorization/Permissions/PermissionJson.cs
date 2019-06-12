using System.Collections.Generic;
using System.Linq;

namespace AbpCompanyName.AbpProjectName.Authorization.Permissions
{
    public class PermissionJson
    {
        public PermissionJson()
        {
            Order = 100;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public int? MultiTenancySide { get; set; }

        public bool DefaultPermission { private get; set; }
        public bool ImportExport { private get; set; }
        public List<PermissionJson> Children { private get; set; }
        public int Order { get; set; }
        public List<int> DisableOrder { get; set; }

        public List<PermissionJson> GetChildren()
        {
            Children = Children ?? new List<PermissionJson>();
            DisableOrder = DisableOrder ?? new List<int>();
            if (DefaultPermission)
            {
                Children.Add(new PermissionJson
                {
                    Name = "List",
                    DisplayName = "列表",
                    Order = 0,
                    DefaultPermission = false
                });
                Children.Add(new PermissionJson
                {
                    Name = "Create",
                    DisplayName = "新增",
                    Order = 10,
                    DefaultPermission = false
                });
                Children.Add(new PermissionJson
                {
                    Name = "Edit",
                    DisplayName = "编辑",
                    Order = 20,
                    DefaultPermission = false
                });
                Children.Add(new PermissionJson
                {
                    Name = "Delete",
                    DisplayName = "删除",
                    Order = 30,
                    DefaultPermission = false
                });
            }
            if (ImportExport)
            {
                Children.Add(new PermissionJson
                {
                    Name = "Import",
                    DisplayName = "导入",
                    Order = 40,
                    DefaultPermission = false
                });
                Children.Add(new PermissionJson
                {
                    Name = "Export",
                    DisplayName = "导出",
                    Order = 50,
                    DefaultPermission = false
                });
            }
            if (MultiTenancySide.HasValue)
            {
                Children.ForEach(c =>
                {
                    c.MultiTenancySide = MultiTenancySide.Value;
                });
            }
            return Children.OrderBy(c => c.Order).Where(c => !DisableOrder.Contains(c.Order)).ToList();
        }
    }
}