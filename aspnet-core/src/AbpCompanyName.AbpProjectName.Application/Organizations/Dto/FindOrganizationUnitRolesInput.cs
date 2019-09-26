using AbpCompanyName.AbpProjectName.Dto;

namespace AbpCompanyName.AbpProjectName.Organizations.Dto
{
    public class FindOrganizationUnitRolesInput : PagedAndFilteredInputDto
    {
        public long OrganizationUnitId { get; set; }
    }
}