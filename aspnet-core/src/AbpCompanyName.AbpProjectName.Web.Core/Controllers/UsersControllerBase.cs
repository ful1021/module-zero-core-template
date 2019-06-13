using Abp.AspNetCore.Mvc.Authorization;
using Abp.BackgroundJobs;
using Abp.UI;
using Abp.Web.Models;
using AbpCompanyName.AbpProjectName.Authorization;
using AbpCompanyName.AbpProjectName.DataExporting.Excel.EpPlus;
using AbpCompanyName.AbpProjectName.Extensions;
using AbpCompanyName.AbpProjectName.Storage;
using AbpCompanyName.AbpProjectName.Users.Importing;
using Microsoft.AspNetCore.Mvc;

namespace AbpCompanyName.AbpProjectName.Controllers
{
    public abstract class UsersControllerBase : AbpProjectNameControllerBase
    {
        protected readonly IBinaryObjectManager BinaryObjectManager;
        protected readonly IBackgroundJobManager BackgroundJobManager;

        protected UsersControllerBase(
            IBinaryObjectManager binaryObjectManager,
            IBackgroundJobManager backgroundJobManager)
        {
            BinaryObjectManager = binaryObjectManager;
            BackgroundJobManager = backgroundJobManager;
        }

        //[HttpPost]
        //[AbpMvcAuthorize(AppPermissions.System_Users_Import)]
        //public async Task<JsonResult> ImportFromExcel()
        //{
        //    try
        //    {
        //        var fileObject = RequestFileToBinaryObject();

        //        await BinaryObjectManager.SaveAsync(fileObject);

        //        await BackgroundJobManager.EnqueueAsync<ImportUsersFromExcelJob, ImportUsersFromExcelJobArgs>(new ImportUsersFromExcelJobArgs
        //        {
        //            TenantId = AbpSession.TenantId,
        //            BinaryObjectId = fileObject.Id,
        //            User = AbpSession.ToUserIdentifier()
        //        });

        //        return Json(new AjaxResponse(new { }));
        //    }
        //    catch (UserFriendlyException ex)
        //    {
        //        return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        //    }
        //}

        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.System_Users_Import)]
        public JsonResult ImportFromExcel()
        {
            try
            {
                var list = GetRequestFileStream().ProcessExcelFile((worksheet, rowIdx) =>
               {
                   var info = new ImportUserDto
                   {
                       UserName = worksheet.GetCheckValue(rowIdx, 1),

                       Name = worksheet.GetCheckValue(rowIdx, 2),

                       Surname = worksheet.TryToString(rowIdx, 3),
                       EmailAddress = worksheet.GetCheckValue(rowIdx, 4, "身份证号", a => !a.CheckIdCard18(), true),

                       PhoneNumber = worksheet.GetCheckValue(rowIdx, 5),
                       Password = worksheet.GetCheckValue(rowIdx, 6),
                       AssignedRoleNames = worksheet.TryToStringArry(7, ',')
                   };

                   return info;
               });

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}