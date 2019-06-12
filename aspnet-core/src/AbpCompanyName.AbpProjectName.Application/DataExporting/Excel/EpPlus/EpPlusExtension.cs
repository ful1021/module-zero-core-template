using System;
using System.Collections.Generic;
using Abp.Collections.Extensions;
using OfficeOpenXml;

namespace AbpCompanyName.AbpProjectName.DataExporting.Excel.EpPlus
{
    public static class EpPlusExtension
    {
        public static void AddHeader(this ExcelWorksheet sheet, params string[] headerTexts)
        {
            if (headerTexts.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < headerTexts.Length; i++)
            {
                AddHeader(sheet, i + 1, headerTexts[i]);
            }
        }

        private static void AddHeader(this ExcelWorksheet sheet, int columnIndex, string headerText)
        {
            sheet.Cells[1, columnIndex].Value = headerText;
            sheet.Cells[1, columnIndex].Style.Font.Bold = true;
        }

        public static void AddObjects<T>(this ExcelWorksheet sheet, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            AddObjects(sheet, 2, items, propertySelectors);
        }

        public static void AddObjects<T>(this ExcelWorksheet sheet, int startRowIndex, IList<T> items, params Func<T, object>[] propertySelectors)
        {
            if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
            {
                return;
            }

            for (var i = 0; i < items.Count; i++)
            {
                for (var j = 0; j < propertySelectors.Length; j++)
                {
                    sheet.Cells[i + startRowIndex, j + 1].Value = propertySelectors[j](items[i]);
                }
            }
        }

        public static ExcelWorksheet AddWorksheets(this ExcelPackage excelPackage, string sheetName = "Sheet1", bool outLineApplyStyle = true)
        {
            var sheet = excelPackage.Workbook.Worksheets.Add(sheetName);
            sheet.OutLineApplyStyle = outLineApplyStyle;
            return sheet;
        }

        public static void AutoFit(this ExcelWorksheet sheet)
        {
            for (var i = 1; i <= sheet.Dimension.Columns; i++)
            {
                sheet.Column(i).AutoFit();
            }
        }
    }
}