using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Abp.IO.Extensions;
using Abp.UI;
using AbpCompanyName.AbpProjectName.Storage;
using ExcelDataReader;
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

        //protected List<TEntity> RequestFileToList<TEntity>(Func<ExcelWorksheet, int, TEntity> processExcelRow)
        //{
        //    //todo @fuliang 使用Npoi 支持xls
        //    return GetRequestFileStream().ProcessExcelFile(processExcelRow);
        //}

        protected List<TEntity> RequestFileToList<TEntity>(Func<IExcelDataReader, int, TEntity> processExcelRow)
        {
            //https://github.com/ExcelDataReader/ExcelDataReader

            var entities = new List<TEntity>();
            var stream = GetRequestFileStream();
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                do
                {
                    var rowIndex = 1;
                    while (reader.Read())
                    {
                        var info = processExcelRow(reader, rowIndex++);
                        if (info == null)
                        {
                            continue;
                        }
                        entities.Add(info);
                    }
                } while (reader.NextResult());
            }

            return entities;
        }
    }
}