using System;
using System.Data;
using Abp.Dependency;
using AbpCompanyName.AbpProjectName.Dto;
using AbpCompanyName.AbpProjectName.Net.MimeTypes;
using AbpCompanyName.AbpProjectName.Storage;
using OfficeOpenXml;

namespace AbpCompanyName.AbpProjectName.DataExporting.Excel.EpPlus
{
    public abstract class EpPlusExcelExporterBase : AbpProjectNameServiceBase, ITransientDependency
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;

        protected EpPlusExcelExporterBase(ITempFileCacheManager tempFileCacheManager)
        {
            _tempFileCacheManager = tempFileCacheManager;
        }

        protected ExcelWorksheet CreateSheet(ExcelPackage excelPackage, DataTable table, string sheetName = "Sheet1", bool outLineApplyStyle = true)
        {
            var sheet = excelPackage.AddWorksheets(sheetName, outLineApplyStyle);

            sheet.Cells["A1"].LoadFromDataTable(table, true);

            return sheet;
        }

        protected ExcelWorksheet CreateSheet(ExcelPackage excelPackage, Action<ExcelWorksheet> creator, string sheetName = "Sheet1", bool outLineApplyStyle = true)
        {
            var sheet = excelPackage.AddWorksheets(sheetName, outLineApplyStyle);

            creator(sheet);

            sheet.AutoFit();

            return sheet;
        }

        protected FileDto CreateExcelPackage(string fileName, Action<ExcelPackage> creator)
        {
            var file = new FileDto(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);

            using (var excelPackage = new ExcelPackage())
            {
                creator(excelPackage);
                Save(excelPackage, file);
            }

            return file;
        }

        protected void Save(ExcelPackage excelPackage, FileDto file)
        {
            _tempFileCacheManager.SetFile(file.FileToken, excelPackage.GetAsByteArray());
        }
    }
}