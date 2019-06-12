using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.BackgroundJobs;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using AbpCompanyName.AbpProjectName.Authorization;
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

        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.System_Users_Import)]
        public async Task<JsonResult> ImportFromExcel()
        {
            try
            {
                var fileObject = RequestFileToBinaryObject();

                await BinaryObjectManager.SaveAsync(fileObject);

                await BackgroundJobManager.EnqueueAsync<ImportUsersFromExcelJob, ImportUsersFromExcelJobArgs>(new ImportUsersFromExcelJobArgs
                {
                    TenantId = AbpSession.TenantId,
                    BinaryObjectId = fileObject.Id,
                    User = AbpSession.ToUserIdentifier()
                });

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.System_Users_Import)]
        public JsonResult ImportFromExcel1()
        {
            try
            {
                var list = CheckTableToList((row, rowIdx) =>
                {
                    var info = new ImportUserDto
                    {
                        UserName = row.GetValue(rowIdx, 0),

                        Name = row.GetValue(rowIdx, 1),

                        Surname = row.TryToString(2),
                        EmailAddress = row.GetValue(rowIdx, 3, "身份证号", a => !a.CheckIdCard18(), true),

                        PhoneNumber = row.GetValue(rowIdx, 4),
                        Password = row.GetValue(rowIdx, 5),
                        AssignedRoleNames = row.TryToStringArry(6, ',')
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