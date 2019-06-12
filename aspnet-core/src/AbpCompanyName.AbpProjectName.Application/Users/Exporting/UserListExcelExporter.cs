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

        //public FileDto ToFile(DataSet ds)
        //{
        //    if (ds == null || ds.Tables.Count <= 0)
        //    {
        //        return null;
        //    }
        //    return CreateExcelPackage("UserList.xlsx", excelPackage =>
        //    {
        //        DataTable osBillDt = ds.Tables[0];
        //        DataTable osDeliveryDt = ds.Tables[1];

        //        CreateSheet(excelPackage, osBillDt, "对账单");
        //        CreateSheet(excelPackage, osDeliveryDt, "发货明细");
        //    });
        //}

        //public FileDto ToFile(DataTable dt)
        //{
        //    return CreateExcelPackage("UserList.xlsx", excelPackage =>
        //    {
        //        CreateSheet(excelPackage, dt, "用户");
        //    });
        //}

        public FileDto ExportToFile(List<UserDto> userListDtos)
        {
            return CreateExcelPackage("UserList.xlsx", excelPackage =>
            {
                var sheet = CreateSheet(excelPackage, st =>
                {
                    string[] headerTexts = new[] { "名字", "姓氏", "用户名", "电话号码", "邮箱地址", "邮箱地址验认", "角色", "激活", "创建时间" };

                    st.AddHeader(headerTexts);

                    st.AddObjects(userListDtos,
                        _ => _.Name,
                        _ => _.Surname,
                        _ => _.UserName,
                        _ => _.PhoneNumber,
                        _ => _.EmailAddress,
                        _ => _.RoleNames?.JoinAsString(", "),
                        _ => _.IsActive,
                        _ => _timeZoneConverter.Convert(_.CreationTime, _abpSession.TenantId, _abpSession.GetUserId())
                        );
                }, "用户");

                //Formatting cells

                var creationTimeColumn = sheet.Column(9);
                creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";
            });
        }
    }
}