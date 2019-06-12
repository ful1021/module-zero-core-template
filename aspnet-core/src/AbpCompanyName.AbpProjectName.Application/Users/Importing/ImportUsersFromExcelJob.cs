using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.ObjectMapping;
using AbpCompanyName.AbpProjectName.Authorization.Users;
using AbpCompanyName.AbpProjectName.Storage;

namespace AbpCompanyName.AbpProjectName.Users.Importing
{
    public class ImportUsersFromExcelJob : BackgroundJob<ImportUsersFromExcelJobArgs>, ITransientDependency
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ILocalizationSource _localizationSource;
        private readonly IObjectMapper _objectMapper;

        public UserManager UserManager { get; set; }

        public ImportUsersFromExcelJob(
            IBinaryObjectManager binaryObjectManager,
            ILocalizationManager localizationManager,
            IObjectMapper objectMapper)
        {
            _binaryObjectManager = binaryObjectManager;
            _objectMapper = objectMapper;
            _localizationSource = localizationManager.GetSource(AbpProjectNameConsts.LocalizationSourceName);
        }

        [UnitOfWork]
        public override void Execute(ImportUsersFromExcelJobArgs args)
        {
            using (CurrentUnitOfWork.SetTenantId(args.TenantId))
            {
                //var users = GetUserListFromExcelOrNull(args);
                //if (users == null || !users.Any())
                //{
                //    SendInvalidExcelNotification(args);
                //    return;
                //}

                //CreateUsers(args, users);
            }
        }
    }
}