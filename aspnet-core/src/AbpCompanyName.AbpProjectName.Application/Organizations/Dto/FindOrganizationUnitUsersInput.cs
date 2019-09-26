using AbpCompanyName.AbpProjectName.Dto;

namespace AbpCompanyName.AbpProjectName.Organizations.Dto
{
    public class FindOrganizationUnitUsersInput : PagedAndFilteredInputDto
    {
        public long OrganizationUnitId { get; set; }
    }
}
