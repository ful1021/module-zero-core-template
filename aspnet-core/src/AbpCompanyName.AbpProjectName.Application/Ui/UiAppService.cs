using System.Collections.Generic;
using AbpCompanyName.AbpProjectName.Authorization;
using AbpCompanyName.AbpProjectName.Ui.Dto;

namespace AbpCompanyName.AbpProjectName.Ui
{
    public class UiAppService : AbpProjectNameAppServiceBase
    {
        public List<ElementUiTableColumnDto> GetColumns(string name)
        {
            List<ElementUiTableColumnDto> elementUis = new List<ElementUiTableColumnDto>();
            switch (name)
            {
                case PermissionNames.System_Users:
                    elementUis = System_Users();
                    break;

                default:
                    break;
            }
            return elementUis;
        }

        private List<ElementUiTableColumnDto> System_Users()
        {
            List<ElementUiTableColumnDto> elementUis = new List<ElementUiTableColumnDto>();
            elementUis.Add(new ElementUiTableColumnDto("用户名", "userName"));
            elementUis.Add(new ElementUiTableColumnDto("名字", "name"));
            elementUis.Add(new ElementUiTableColumnDto("姓氏", "surname"));
            elementUis.Add(new ElementUiTableColumnDto("角色", "role", sortable: false));
            elementUis.Add(new ElementUiTableColumnDto("邮箱地址", "emailAddress"));
            elementUis.Add(new ElementUiTableColumnDto("邮箱地址验认", "isEmailConfirmed", "tag", false, true, "140px", CssAlign.center));
            elementUis.Add(new ElementUiTableColumnDto("激活", "isActive", "tag", false, true, "140px", CssAlign.center));
            elementUis.Add(new ElementUiTableColumnDto("创建时间", "creationTime"));
            return elementUis;
        }
    }
}