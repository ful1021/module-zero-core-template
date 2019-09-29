using System;
using Abp.GeneralTree;

namespace AbpCompanyName.AbpProjectName.Organizations.Dto
{

    public class OrganizationUnitDto : GeneralTreeDto<OrganizationUnitDto, long>
    {
        public DateTime CreationTime { get; set; }
        public long? CreatorUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public string DisplayName { get; set; }

        public int MemberCount { get; set; }

        public int RoleCount { get; set; }
    }
}