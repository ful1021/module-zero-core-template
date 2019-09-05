using Abp.Application.Services.Dto;

namespace AbpCompanyName.AbpProjectName.Authorization.Users.Dto
{
    public class UserListInput : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}