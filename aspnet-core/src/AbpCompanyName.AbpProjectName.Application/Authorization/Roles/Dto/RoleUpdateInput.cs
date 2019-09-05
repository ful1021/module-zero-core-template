using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace AbpCompanyName.AbpProjectName.Authorization.Roles.Dto
{
    public class RoleUpdateInput : EntityDto, IValidatableObject
    {
        [Required]
        public string DisplayName { get; set; }

        public bool IsDefault { get; set; }

        [Required]
        public List<string> GrantedPermissionNames { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Id <= 0)
            {
                yield return new ValidationResult("角色Id有误", new[] { "Id" });
            }
        }
    }
}