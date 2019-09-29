using System.Collections.Generic;

namespace AbpCompanyName.AbpProjectName.Sessions.Dto
{
    public class GetCurrentLoginInformationsOutput
    {
        public ApplicationInfoDto Application { get; set; }

        public UserLoginInfoDto User { get; set; }

        public TenantLoginInfoDto Tenant { get; set; }

        public object Session { get; set; }
        public object GrantedPermissions { get; set; }
        public IList<string> Roles { get; set; }
    }
}