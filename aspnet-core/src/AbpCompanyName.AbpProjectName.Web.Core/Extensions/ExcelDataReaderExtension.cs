using System;
using System.Linq;
using Abp.UI;
using ExcelDataReader;

namespace AbpCompanyName.AbpProjectName.Extensions
{
    public static class ExcelDataReaderExtension
    {
        #region GetValue

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
        public static string GetCheckValue(this IExcelDataReader worksheet, int rowIndex, int columnIndex, string errorMsg = "", Func<string, bool> customValidateFunc = null, bool isNullable = false)
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
        public static string TryToString(this IExcelDataReader worksheet, int rowIndex, int columnIndex, string value = "")
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
        public static string[] TryToStringArry(this IExcelDataReader worksheet, int rowIndex, int columnIndex, params char[] separator)
        {
            object val = GetCellsVal(worksheet, rowIndex, columnIndex);
            var str = TryToString(val, "");
            return str.Split(separator).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
        }

        private static object GetCellsVal(this IExcelDataReader worksheet, int rowIndex, int columnIndex)
        {
            return worksheet.GetValue(columnIndex - 1);
        }

        private static string TryToString(object val, string value)
        {
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()) || val.ToString().ToLower() == "null")
            {
                return value;
            }
            return val.ToString();
        }

        #endregion GetValue
    }
}