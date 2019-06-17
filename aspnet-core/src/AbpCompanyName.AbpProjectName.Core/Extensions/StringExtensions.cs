using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Abp.Extensions;
using Abp.Timing;

namespace AbpCompanyName.AbpProjectName.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 字符串分割成字符串组
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <param name="splitChars"></param>
        /// <returns></returns>
        public static string[] ToStringArray(this string sourceStr, char[] splitChars = null)
        {
            if (sourceStr.IsNullOrEmpty())
            {
                return new string[] { };
            }
            splitChars = splitChars ?? new char[] { ' ', '\r', '\n', '\t', '，', ',', '、', ';', '；' };

            //排除String数组中空格项
            return sourceStr
                .Trim()
                .Split(splitChars, System.StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// 去掉字符串两端 \r \n \t 等特殊字符
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static string ToSafeString(this string sourceStr)
        {
            if (sourceStr.IsNullOrWhiteSpace())
            {
                return sourceStr;
            }
            var removeChars = new char[] { ' ', '\r', '\n', '\'', ',', '，', '\t', '\"' };

            return sourceStr.Trim(removeChars);
        }

        /// <summary>
        /// 将字符串（-分割） 拆分为时间范围
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static DateTimeRange ToDateTimeRange(this string str, string separator = " - ")
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                var temp = str.Trim().Split(separator);
                if (temp.Length >= 2)
                {
                    var StartTime = $"{temp[0].Trim()} 00:00:00".TryToDateTimeOrNull();
                    var EndTime = $"{temp[1].Trim()} 23:59:59".TryToDateTimeOrNull();
                    if (StartTime.HasValue && EndTime.HasValue)
                    {
                        return new DateTimeRange
                        {
                            StartTime = StartTime.Value,
                            EndTime = EndTime.Value
                        };
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 字符串转换为DateTime类型或者null，不成功返回null
        /// </summary>
        public static DateTime? TryToDateTimeOrNull(this string str)
        {
            DateTime? result = null;
            if (!string.IsNullOrWhiteSpace(str))
            {
                DateTime tmp;
                if (DateTime.TryParse(str, out tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 字符串转换为DateTime类型或者null，不成功返回null
        /// </summary>
        public static DateTime TryToDateTime(this string str, string defaultResult = "1900-01-01")
        {
            var result = str.TryToDateTimeOrNull();
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
        /// 字符串转换为 int 类型
        /// </summary>
        public static int TryToInt32(this string str, int defaultResult = 0)
        {
            var result = defaultResult;
            if (!string.IsNullOrWhiteSpace(str))
            {
                int tmp;
                if (int.TryParse(str, out tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 字符串转换为 bool 类型
        /// </summary>
        public static bool TryToBool(this string str, bool defaultResult = false)
        {
            var result = defaultResult;
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (bool.TryParse(str, out bool tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        /// <summary>
        /// 字符串转换为 decimal 类型
        /// </summary>
        public static decimal TryToDecimal(this string str, decimal defaultResult = 0)
        {
            decimal result = defaultResult;
            if (!string.IsNullOrWhiteSpace(str))
            {
                decimal tmp;
                if (decimal.TryParse(str, out tmp))
                {
                    result = tmp;
                }
            }
            return result;
        }

        public static string UrlEncode(this string sourceStr)
        {
            var str = System.Net.WebUtility.UrlEncode(sourceStr) ?? sourceStr;

            var pattern = @"%\w{2}";
            var matchArray = Regex.Matches(str, pattern);

            foreach (var match in matchArray)
            {
                var matchStr = match.ToString();
                str = str.Replace(matchStr, matchStr.ToLower());
            }

            return str;
        }

        public static string UrlDecode(this string sourceStr)
        {
            return System.Net.WebUtility.UrlDecode(sourceStr);
        }

        /// <summary>
        /// 字符串中的中文，转换为 Unicode，非中文跳过
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUnicode(this string str)
        {
            StringBuilder strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    var val = (int)str[i];
                    if (val > 127)
                    {
                        strResult.Append("\\u");

                        strResult.Append(val.ToString("x"));
                    }
                    else
                    {
                        strResult.Append(str[i]);
                    }
                }
            }
            return strResult.ToString();
        }

        /// <summary>
        /// 处理字符串，对显示字符串稍作加密，比身份证号，手机号、银行卡号等，只显示前几位和最后几位，其余部分均显示*
        /// </summary>
        /// <param name="str"></param>
        /// <param name="frontLength">前几位</param>
        /// <param name="backLength">后几位</param>
        /// <returns></returns>
        public static string MaskStr(this string str, int frontLength = 3, int backLength = 3)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (frontLength >= str.Length)
                {
                    frontLength = str.Length;
                }
                sb.Append(str.Substring(0, frontLength));
                for (int i = 0; i < str.Length - frontLength - backLength; i++)
                {
                    sb.Append("*");
                }
                if (backLength >= str.Length)
                {
                    backLength = str.Length;
                }
                sb.Append(str.Substring(str.Length - backLength));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 处理邮箱，对显示邮箱稍作加密，比如abcd@163.com，处理之后变为a***@163.com
        /// </summary>
        /// <param name="mail">邮箱</param>
        ///  <param name="length">显示长度</param>
        /// <returns></returns>
        public static string MaskMail(this string mail, int length = 1)
        {
            StringBuilder sb = new StringBuilder();
            string str = "";
            for (int j = 0; j < length; j++)
            {
                str += @"\w";
            }
            if (!string.IsNullOrWhiteSpace(mail))
            {
                Match match = Regex.Match(mail, @"^(" + str + @")(\w*)@(.+)$");
                string first = match.Groups[1].Value;
                string second = match.Groups[2].Value;
                string domain = match.Groups[3].Value;
                sb.Append(first);
                for (int i = 0; i < second.Length; i++)
                {
                    sb.Append("*");
                }
                sb.Append("@");
                sb.Append(domain);
            }
            return sb.ToString();
        }
    }
}