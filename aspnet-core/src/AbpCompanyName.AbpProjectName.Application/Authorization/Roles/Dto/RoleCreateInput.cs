using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.Authorization.Roles.Dto
{
    public class RoleCreateInput
    {
        [Required]
        public string DisplayName { get; set; }

        public bool IsDefault { get; set; }

        [Required]
        public List<string> GrantedPermissionNames { get; set; }
    }
}