using System;
using System.Data;
using System.Linq;
using Abp.UI;

namespace AbpCompanyName.AbpProjectName.Extensions
{
    /// <summary>
    /// DataRow 扩展方法
    /// </summary>
    public static class DataRowExtension
    {
        /// <summary>
        /// 检验 并得到值
        /// </summary>
        /// <param name="row"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="errorMsg"></param>
        /// <param name="customValidateFunc">自定义验证方法</param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public static string GetCheckValue(this DataRow row, int rowIndex, int columnIndex, string errorMsg = "", Func<string, bool> customValidateFunc = null, bool isNullable = false)
        {
            var val = row[columnIndex].ToString().Trim();
            if (!isNullable)
            {
                if (string.IsNullOrWhiteSpace(val))
                {
                    throw new UserFriendlyException($"对比模板，检查第 {columnIndex + 1} 列，第 {rowIndex} 行 {errorMsg} 值有误");
                }
            }
            if (!string.IsNullOrWhiteSpace(val))
            {
                if (customValidateFunc?.Invoke(val) ?? false)
                {
                    throw new UserFriendlyException($"对比模板，检查第 {columnIndex + 1} 列，第 {rowIndex} 行 {errorMsg} 值为 {val} 有误");
                }
            }
            return val;
        }

        public static string TryToString(this DataRow row, int columnIdx, string value = "")
        {
            object val = GetDataRowVal(row, columnIdx);
            return TryToString(val, value);
        }

        public static string TryToString(this DataRow row, string columnIdx, string value = "")
        {
            object val = row[columnIdx];
            return TryToString(val, value);
        }

        public static string[] TryToStringArry(this DataRow row, int columnIdx, params char[] separator)
        {
            object val = row[columnIdx];
            var str = TryToString(val, "");
            return str.Split(separator).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();
        }

        public static Guid TryToGuid(this DataRow row, int columnIdx, Guid value)
        {
            object val = GetDataRowVal(row, columnIdx);
            return TryToGuid(val, value);
        }

        public static Guid TryToGuid(this DataRow row, string columnIdx, Guid value)
        {
            object val = row[columnIdx];
            return TryToGuid(val, value);
        }

        public static Guid TryToGuid(this DataRow row, string columnIdx)
        {
            return row.TryToGuid(columnIdx, Guid.Empty);
        }

        public static decimal TryToDecimal(this DataRow row, int columnIdx, decimal value = 0)
        {
            object val = GetDataRowVal(row, columnIdx);
            return TryToDecimal(val, value);
        }

        public static int TryToInt32(this DataRow row, int columnIdx, int value = 0)
        {
            object val = GetDataRowVal(row, columnIdx);
            return TryToInt32(val, value);
        }

        public static int TryToInt32(this DataRow row, string columnIdx, int value = 0)
        {
            object val = row[columnIdx];
            return TryToInt32(val, value);
        }

        public static DateTime TryToDateTime(this DataRow row, int columnIdx, DateTime value)
        {
            object val = GetDataRowVal(row, columnIdx);
            return TryToDateTime(val, value);
        }

        public static DateTime? TryToDateTimeOrNull(this DataRow row, int columnIdx)
        {
            object val = GetDataRowVal(row, columnIdx);
            return TryToDateTimeOrNull(val);
        }

        public static DateTime TryToDateTime(this DataRow row, string columnIdx, DateTime value)
        {
            object val = row[columnIdx];
            return TryToDateTime(val, value);
        }

        public static DateTime TryToDateTime(this DataRow row, int columnIdx, string value)
        {
            return row.TryToDateTime(columnIdx, Convert.ToDateTime(value));
        }

        public static DateTime TryToDatetime(this DataRow row, string columnIdx, string value)
        {
            return row.TryToDateTime(columnIdx, Convert.ToDateTime(value));
        }

        #region private

        private static object GetDataRowVal(DataRow row, int columnIdx)
        {
            if (row.Table.Columns.Count > columnIdx)
            {
                return row[columnIdx];
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

        private static Guid TryToGuid(object val, Guid value)
        {
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()) || val.ToString().ToLower() == "null")
            {
                return value;
            }
            Guid dt;
            if (Guid.TryParse(val.ToString(), out dt))
            {
                value = dt;
            }
            return value;
        }

        private static int TryToInt32(object val, int value = 0)
        {
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()) || val.ToString().ToLower() == "null")
            {
                return value;
            }
            int dt;
            if (int.TryParse(val.ToString(), out dt))
            {
                value = dt;
            }
            return value;
        }

        private static decimal TryToDecimal(object val, decimal value = 0)
        {
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()) || val.ToString().ToLower() == "null")
            {
                return value;
            }
            decimal dt;
            if (decimal.TryParse(val.ToString(), out dt))
            {
                value = dt;
            }
            return value;
        }

        public static DateTime? TryToDateTimeOrNull(object val, DateTime? value = null)
        {
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()) || val.ToString().ToLower() == "null")
            {
                return value;
            }
            DateTime dt;
            if (DateTime.TryParse(val.ToString(), out dt))
            {
                value = dt;
            }
            return value;
        }

        private static DateTime TryToDateTime(object val, DateTime value)
        {
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()) || val.ToString().ToLower() == "null")
            {
                return value;
            }
            DateTime dt;
            if (DateTime.TryParse(val.ToString(), out dt))
            {
                value = dt;
            }
            return value;
        }

        #endregion private
    }
}