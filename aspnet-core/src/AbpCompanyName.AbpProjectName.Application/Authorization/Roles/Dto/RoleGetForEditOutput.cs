using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AbpCompanyName.AbpProjectName.Authorization.Roles.Dto
{
    public class RoleGetForEditOutput
    {
        public int? Id { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public bool IsDefault { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}