using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Abp.IO.Extensions;
using Abp.UI;
using AbpCompanyName.AbpProjectName.DataExporting.Excel.EpPlus;
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

        protected DataTable RequestFileToDataTable()
        {
            using (var stream = GetRequestFileStream())
            {
                var dt = stream.ToDataTable();
                if (dt == null || dt.Rows.Count <= 0)
                {
                    throw new UserFriendlyException("文件不存在数据");
                }
                return dt;
            }
        }

        protected List<T> CheckTableToList<T>(Func<DataRow, int, T> addList, DataTable dataTable = null)
        {
            var dt = dataTable ?? RequestFileToDataTable();

            List<T> list = new List<T>();
            var rowIndex = 1;
            foreach (DataRow row in dt.Rows)
            {
                rowIndex++;
                list.Add(addList(row, rowIndex));
            }
            return list;
        }
    }
}