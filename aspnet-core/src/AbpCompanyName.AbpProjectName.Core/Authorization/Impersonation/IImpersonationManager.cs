using System.Threading.Tasks;
using Abp.Domain.Services;

namespace AbpCompanyName.AbpProjectName.Authorization.Impersonation
{
    public interface IImpersonationManager : IDomainService
    {
        Task<UserAndIdentity> GetImpersonatedUserAndIdentity(string impersonationToken);

        Task<string> GetImpersonationToken(long userId, int? tenantId);

        Task<string> GetBackToImpersonatorToken();
    }
}
