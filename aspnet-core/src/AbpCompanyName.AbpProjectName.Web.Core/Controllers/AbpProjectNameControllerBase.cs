using System.IO;
using System.Linq;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Abp.IO.Extensions;
using Abp.UI;
using AbpCompanyName.AbpProjectName.Storage;
using Microsoft.AspNetCore.Identity;

namespace AbpCompanyName.AbpProjectName.Controllers
{
    public abstract class AbpProjectNameControllerBase : AbpController
    {
        protected AbpProjectNameControllerBase()
        {
            LocalizationSourceName = AbpProjectNameConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        protected Stream GetRequestFileStream()
        {
            if (Request.Form.Files == null || Request.Form.Files.Count <= 0)
            {
                throw new UserFriendlyException("不存在文件");
            }
            var file = Request.Form.Files.First();

            if (file == null || file.Length <= 0)
            {
                throw new UserFriendlyException("文件不能为空");
            }

            if (file.Length > 1048576 * 100) //100 MB
            {
                throw new UserFriendlyException("文件太大");
            }
            var stream = file.OpenReadStream();
            return stream;
        }

        protected BinaryObject RequestFileToBinaryObject()
        {
            byte[] fileBytes;
            using (var stream = GetRequestFileStream())
            {
                fileBytes = stream.GetAllBytes();
            }
            var fileObject = new BinaryObject(AbpSession.TenantId, fileBytes);
            return fileObject;
        }
    }
}