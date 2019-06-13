using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.UI;
using OfficeOpenXml;

namespace AbpCompanyName.AbpProjectName.DataExporting.Excel.EpPlus
{
    public static class EpPlusExtension
    {
        #region ExcelToDataTable

        public static DataTable ToDataTable(this Stream stream)
        {
            using (var pck = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = pck.Workbook.Worksheets[1];
                return ExcelPackageToDataTable(pck, worksheet);
            }
        }

        public static DataSet ToDataSet(this Stream stream)
        {
            using (var pck = new ExcelPackage(stream))
            {
                DataSet ds = new DataSet();
                foreach (var item in pck.Workbook.Worksheets)
                {
                    var dt = ExcelPackageToDataTable(pck, item);
                    dt.TableName = item.Name;
                    ds.Tables.Add(dt);
                }
                return ds;
            }
        }

        private static DataTable ExcelPackageToDataTable(this ExcelPackage excelPackage, ExcelWorksheet worksheet)
        {
            DataTable dt = new DataTable();

            //check if the worksheet is completely empty
            var rowCount = worksheet.Dimension?.Rows;
            var colCount = worksheet.Dimension?.Columns;

            if (!rowCount.HasValue || !colCount.HasValue)
            {
                return dt;
            }

            //create a list to hold the column names
            List<string> columnNames = new List<string>();

            //needed to keep track of empty column headers
            int currentColumn = 1;

            //loop all columns in the sheet and add them to the datatable
            foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
            {
                string columnName = cell.Text.Trim();

                //check if the previous header was empty and add it if it was
                if (cell.Start.Column != currentColumn)
                {
                    columnNames.Add("Header_" + currentColumn);
                    dt.Columns.Add("Header_" + currentColumn);
                    currentColumn++;
                }

                //add the column name to the list to count the duplicates
                columnNames.Add(columnName);

                //count the duplicate column names and make them unique to avoid the exception
                //A column named 'Name' already belongs to this DataTable
                int occurrences = columnNames.Count(x => x.Equals(columnName));
                if (occurrences > 1)
                {
                    columnName = columnName + "_" + occurrences;
                }

                //add the column to the datatable
                dt.Columns.Add(columnName);

                currentColumn++;
            }

            //start adding the contents of the excel file to the datatable
            for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
            {
                var row = worksheet.Cells[i, 1, i, worksheet.Dimension.End.Column];
                DataRow newRow = dt.NewRow();

                //loop all cells in the row
                foreach (var cell in row)
                {
                    newRow[cell.Start.Column - 1] = cell.Text;
                }

                dt.Rows.Add(newRow);
            }

            return dt;
        }

        #endregion ExcelToDataTable

        #region ExcelToList

        public static List<T> ProcessExcelFile<T>(this Stream stream, Func<ExcelWorksheet, int, T> processExcelRow)
        {
            var entities = new List<T>();

            using (stream)
            {
                using (var excelPackage = new ExcelPackage(stream))
                {
                    foreach (var worksheet in excelPackage.Workbook.Worksheets)
                    {
                        var entitiesInWorksheet = ProcessWorksheet(worksheet, processExcelRow);

                        entities.AddRange(entitiesInWorksheet);
                    }
                }
            }

            return entities;
        }

        private static List<T> ProcessWorksheet<T>(ExcelWorksheet worksheet, Func<ExcelWorksheet, int, T> processExcelRow)
        {
            var entities = new List<T>();

            for (var i = worksheet.Dimension.Start.Row + 1; i <= worksheet.Dimension.End.Row; i++)
            {
                try
                {
                    if (worksheet.IsRowEmpty(i))
                    {
                        continue;
                    }
                    var entity = processExcelRow(worksheet, i);

                    if (entity != null)
                    {
                        entities.Add(entity);
                    }
                }
                catch (Exception)
                {
                    //ignore
                }
            }

            return entities;
        }

        #endregion ExcelToList

        #region ToExcel

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

        #endregion ToExcel

        public static bool IsRowEmpty(this ExcelWorksheet worksheet, int row)
        {
            return worksheet.Cells[row, 1].Value == null || string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Value.ToString());
        }

        /// <summary>
        /// 检验 并得到值
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="rowIndex">从1开始</param>
        /// <param name="columnIndex">从1开始</param>
        /// <param name="errorMsg"></param>
        /// <param name="customValidateFunc">自定义验证方法</param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public static string GetCheckValue(this ExcelWorksheet worksheet, int rowIndex, int columnIndex, string errorMsg = "", Func<string, bool> customValidateFunc = null, bool isNullable = false)
        {
            object val = GetCellsVal(worksheet, rowIndex, columnIndex);
            var strVal = val?.ToString().Trim();
            if (!isNullable)
            {
                if (string.IsNullOrWhiteSpace(strVal))
                {
                    throw new UserFriendlyException($"对比模板，检查第 {columnIndex } 列，第 {rowIndex} 行 {errorMsg} 值不能为空");
                }
            }
            if (!string.IsNullOrWhiteSpace(strVal))
            {
                if (customValidateFunc?.Invoke(strVal) ?? false)
                {
                    throw new UserFriendlyException($"对比模板，检查第 {columnIndex } 列，第 {rowIndex} 行 {errorMsg} 值为 {strVal} 有误");
                }
            }
            return strVal;
        }

        /// <summary>
        /// 获取单元格值 转String
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="rowIndex">从1开始</param>
        /// <param name="columnIndex">从1开始</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TryToString(this ExcelWorksheet worksheet, int rowIndex, int columnIndex, string value = "")
        {
            object val = GetCellsVal(worksheet, rowIndex, columnIndex);
            return TryToString(val, value);
        }

        /// <summary>
        /// 获取单元格值 转String[]
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="rowIndex">从1开始</param>
        /// <param name="columnIndex">从1开始</param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] TryToStringArry(this ExcelWorksheet worksheet, int rowIndex, int columnIndex, params char[] separator)
        {
            object val = GetCellsVal(worksheet, rowIndex, columnIndex);
            var str = TryToString(val, "");
            return str.Split(separator).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
        }

        private static object GetCellsVal(this ExcelWorksheet worksheet, int rowIndex, int columnIndex)
        {
            if (worksheet.Cells.Columns >= columnIndex && worksheet.Cells.Rows >= rowIndex)
            {
                return worksheet.Cells[rowIndex, columnIndex].Value;
            }
            return null;
        }

        private static string TryToString(object val, string value)
        {
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()) || val.ToString().ToLower() == "null")
            {
                return value;
            }
            return val.ToString();
        }
    }
}