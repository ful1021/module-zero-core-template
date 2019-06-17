using System;
using System.Text.RegularExpressions;
using Abp.Extensions;

namespace AbpCompanyName.AbpProjectName.Validation
{
    public static class ValidationHelper
    {
        public const string EmailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        public static bool IsEmail(this string value)
        {
            if (value.IsNullOrEmpty())
            {
                return false;
            }

            var regex = new Regex(EmailRegex);
            return regex.IsMatch(value);
        }

        /// <summary>
        /// 是否是手机号
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsMobile(this string mobile)
        {
            if (!string.IsNullOrWhiteSpace(mobile))
            {
                string reg = @"^((13|14|15|16|17|18|19)+\d{9})$";
                Regex dReg = new Regex(reg);
                var b = dReg.IsMatch(mobile);
                return b;
            }
            return false;
        }

        /// <summary>
        /// 是否为url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsUrl(this string url)
        {
            if (url.IsNullOrEmpty()) return false;

            try
            {
                var isUrl = Regex.IsMatch(url,
                    @"\b(?:(?:https?|ftp|file)://|www\.|ftp\.)[-A-Ö0-9+&@#/%=~_|$?!:,.]*[A-Ö0-9+&@#/%=~_|$]",
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                return isUrl;
            }
            catch
            {
                return false;
            }
        }

        public static bool RegexIsMatch(this string str, string regStr)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                Regex reg = new Regex(regStr);
                return reg.IsMatch(str);
            }
            return false;
        }

        /// <summary>
        /// 18位身份证号码验证
        /// </summary>
        public static bool CheckIdCard18(this string idNumber)
        {
            if (idNumber.Length < 18)
            {
                return false;
            }
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2), StringComparison.Ordinal) == -1)
            {
                return false;//省份验证
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time;
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(wi[i]) * int.Parse(ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }
    }
}