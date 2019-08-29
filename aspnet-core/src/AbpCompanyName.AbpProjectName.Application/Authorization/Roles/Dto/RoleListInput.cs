using Abp.Application.Services.Dto;

namespace AbpCompanyName.AbpProjectName.Authorization.Roles.Dto
{
    public class RoleListInput : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }

        public string Permission { get; set; }
    }
}

