using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AbpCompanyName.AbpProjectName.Authorization.Users;

namespace AbpCompanyName.AbpProjectName.Authorization.Impersonation
{
    public class UserAndIdentity
    {
        public User User { get; set; }

        public ClaimsIdentity Identity { get; set; }

        public UserAndIdentity(User user, ClaimsIdentity identity)
        {
            User = user;
            Identity = identity;
        }
    }
}
