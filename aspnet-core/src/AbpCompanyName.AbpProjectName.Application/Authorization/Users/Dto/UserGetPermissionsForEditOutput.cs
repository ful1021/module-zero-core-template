using System.Collections.Generic;
using AbpCompanyName.AbpProjectName.Authorization.Permissions.Dto;

namespace AbpCompanyName.AbpProjectName.Authorization.Users.Dto
{
    public class UserGetPermissionsForEditOutput
    {
        public List<string> GrantedPermissionNames { get; set; }
    }
}