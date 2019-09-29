using Abp.GeneralTree;

namespace AbpCompanyName.AbpProjectName.Organizations.Dto
{
    public class OrganizationUnitByDto : GeneralTreeDto<OrganizationUnitByDto, long>
    {
        public string DisplayName { get; set; }
    }
}