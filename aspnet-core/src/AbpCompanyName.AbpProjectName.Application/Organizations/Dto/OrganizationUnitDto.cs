using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.GeneralTree;

namespace AbpCompanyName.AbpProjectName.Organizations.Dto
{
    public class OrganizationUnitDto : AuditedEntityDto<long>, IGeneralTreeDto<OrganizationUnitDto, long>
    {
        public long? ParentId { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public int MemberCount { get; set; }

        public int RoleCount { get; set; }

        public ICollection<OrganizationUnitDto> Children { get; set; }
    }
}