using System;
using Abp;

namespace AbpCompanyName.AbpProjectName.Users.Exporting
{
    public class ExportUsersToExcelJobArgs
    {
        public int? TenantId { get; set; }

        public Guid BinaryObjectId { get; set; }

        public UserIdentifier User { get; set; }
    }
}