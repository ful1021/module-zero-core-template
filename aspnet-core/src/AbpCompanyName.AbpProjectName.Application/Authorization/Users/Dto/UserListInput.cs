using Abp.Application.Services.Dto;
using AbpCompanyName.AbpProjectName.Dto;

namespace AbpCompanyName.AbpProjectName.Authorization.Users.Dto
{
    public class UserListInput : PagedSortedAndFilteredInputDto
    {
        public bool? IsActive { get; set; }
    }
}