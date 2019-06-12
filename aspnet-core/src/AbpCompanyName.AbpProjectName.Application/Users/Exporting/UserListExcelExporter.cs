using System.Collections.Generic;
using Abp.Collections.Extensions;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using AbpCompanyName.AbpProjectName.DataExporting.Excel.EpPlus;
using AbpCompanyName.AbpProjectName.Dto;
using AbpCompanyName.AbpProjectName.Storage;
using AbpCompanyName.AbpProjectName.Users.Dto;

namespace AbpCompanyName.AbpProjectName.Authorization.Users.Exporting
{
    public class UserListExcelExporter : EpPlusExcelExporterBase
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public UserListExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<UserDto> userListDtos)
        {
            return CreateExcelPackage(
                "UserList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add("Users");
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        "Name",
                        "Surname",
                        "UserName",
                        "PhoneNumber",
                        "EmailAddress",
                        "Roles",
                        "Active",
                        "CreationTime"
                        );

                    AddObjects(
                        sheet, 2, userListDtos,
                        _ => _.Name,
                        _ => _.Surname,
                        _ => _.UserName,
                        _ => _.PhoneNumber,
                        _ => _.EmailAddress,
                        _ => _.RoleNames?.JoinAsString(", "),
                        _ => _.IsActive,
                        _ => _timeZoneConverter.Convert(_.CreationTime, _abpSession.TenantId, _abpSession.GetUserId())
                        );

                    //Formatting cells

                    var creationTimeColumn = sheet.Column(9);
                    creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    for (var i = 1; i <= 9; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                });
        }
    }
}