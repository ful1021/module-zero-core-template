using Abp.AspNetCore.Mvc.Authorization;
using Abp.BackgroundJobs;
using Abp.UI;
using Abp.Web.Models;
using AbpCompanyName.AbpProjectName.Authorization;
using AbpCompanyName.AbpProjectName.Extensions;
using AbpCompanyName.AbpProjectName.Storage;
using AbpCompanyName.AbpProjectName.Users.Importing;
using AbpCompanyName.AbpProjectName.Validation;
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

        //[HttpPost]
        //[AbpMvcAuthorize(AppPermissions.System_Users_Import)]
        //public JsonResult ImportFromExcel()
        //{
        //    try
        //    {
        //        var list = RequestFileToList((worksheet, rowIdx) =>
        //       {
        //           var info = new ImportUserDto
        //           {
        //               UserName = worksheet.GetCheckValue(rowIdx, 1),

        //               Name = worksheet.GetCheckValue(rowIdx, 2),

        //               Surname = worksheet.TryToString(rowIdx, 3),
        //               EmailAddress = reader.GetCheckValue(rowIdx, 4, "邮箱地址", a => !a.IsEmail()),

        //               PhoneNumber = worksheet.GetCheckValue(rowIdx, 5),
        //               Password = worksheet.GetCheckValue(rowIdx, 6),
        //               AssignedRoleNames = worksheet.TryToStringArry(7, ',')
        //           };

        //           return info;
        //       });

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
                var list = RequestFileToList((reader, rowIdx) =>
               {
                   var info = new ImportUserDto
                   {
                       UserName = reader.GetCheckValue(rowIdx, 1),

                       Name = reader.GetCheckValue(rowIdx, 2),

                       Surname = reader.TryToString(rowIdx, 3),
                       EmailAddress = reader.GetCheckValue(rowIdx, 4, "邮箱地址", a => !a.IsEmail()),

                       PhoneNumber = reader.GetCheckValue(rowIdx, 5),
                       Password = reader.GetCheckValue(rowIdx, 6),
                       AssignedRoleNames = reader.TryToStringArry(7, ',')
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