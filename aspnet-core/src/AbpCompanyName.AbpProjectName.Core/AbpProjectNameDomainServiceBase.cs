using Abp.Domain.Services;

namespace AbpCompanyName.AbpProjectName
{
    public abstract class AbpProjectNameDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected AbpProjectNameDomainServiceBase()
        {
            LocalizationSourceName = AbpProjectNameConsts.LocalizationSourceName;
        }
    }
}

