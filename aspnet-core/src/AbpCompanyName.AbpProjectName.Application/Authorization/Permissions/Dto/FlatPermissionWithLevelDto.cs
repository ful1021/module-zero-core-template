using Abp.Authorization;
using Abp.AutoMapper;

namespace AbpCompanyName.AbpProjectName.Authorization.Permissions.Dto
{
    [AutoMap(typeof(Permission))]
    public class FlatPermissionWithLevelDto : FlatPermissionDto
    {
        public int Level { get; set; }
    }
}