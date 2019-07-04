using System;
using System.Linq;
using CodeSmith.Engine;

namespace CodeSmith
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Util
    {
        #region 字符串扩展方法

        /// <summary>
        /// 转换为CamelCase命名法,第一个单词首字母小写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToCamel(string value) => StringUtil.ToCamelCase(value);

        /// <summary>
        /// 转换为PascalCase命名法,第一个单词首字母大写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPascal(string value) => StringUtil.ToPascalCase(value);

        /// <summary>
        /// 转换为单数形式:如Users 生成 User
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSingular(string value) => StringUtil.ToSingular(value);

        /// <summary>
        /// 转换为复数形式:如User 生成 Users
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPlural(string value) => StringUtil.ToPlural(value);

        #endregion 字符串扩展方法

        #region Helper方法

        /// <summary>
        /// 判断item是否存在list中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsIn<T>(T item, params T[] list)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// 判断item是否存在list中
        /// </summary>
        /// <param name="item"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsIn(string item, params string[] list)
        {
            return list.Any(a => item.Equals(a, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// 对象 转换为DateTime类型或者null，不成功返回null
        /// </summary>
        public static DateTime? TryToDateTimeOrNull(object str)
        {
            DateTime? result = null;
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                DateTime tmp;
                if (DateTime.TryParse(str.ToString(), out tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 对象 转换为DateTime类型或者null，不成功返回null
        /// </summary>
        public static DateTime TryToDateTime(object str, string defaultResult = null)
        {
            var result = TryToDateTimeOrNull(str);
            if (result == null && !string.IsNullOrWhiteSpace(defaultResult))
            {
                DateTime tmp;
                if (DateTime.TryParse(defaultResult, out tmp))
                {
                    result = tmp;
                }
            }
            return result ?? DateTime.MinValue;
        }

        /// <summary>
        /// 对象 转换为 decimal 类型
        /// </summary>
        public static decimal TryToDecimal(object str, decimal defaultResult = 0)
        {
            var result = defaultResult;
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                decimal tmp;
                if (decimal.TryParse(str.ToString(), out tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 对象 转换为 int 类型
        /// </summary>
        public static int TryToInt(object str, int defaultResult = 0)
        {
            var result = defaultResult;
            if (str != null && !string.IsNullOrWhiteSpace(str.ToString()))
            {
                if (int.TryParse(str.ToString(), out int tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 对象 转换为 string 类型
        /// </summary>
        public static string TryToString(object source)
        {
            if (source != null)
            {
                return source.ToString();
            }
            return "";
        }

        #endregion Helper方法

        #region 字符串扩展方法

        /// <summary>
        /// 转换为CamelCase命名法,第一个单词首字母小写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToFirstLetterCamel(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            return value[0].ToString().ToLower() + value.Substring(1);
        }

        #endregion 字符串扩展方法
    }
}