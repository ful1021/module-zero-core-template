using System;
using Abp.Timing;

namespace AbpCompanyName.AbpProjectName.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 将日期数组 转为时间范围
        /// </summary>
        /// <param name="dataArr"></param>
        /// <returns></returns>
        public static DateTimeRange ToDateTimeRange(this DateTime[] dataArr)
        {
            if (dataArr != null && dataArr.Length == 2)
            {
                return new DateTimeRange
                {
                    StartTime = dataArr[0],
                    EndTime = dataArr[1]
                };
            }
            return null;
        }

        /// <summary>
        /// 取得日期 对应年月的 第一天日期
        /// </summary>
        /// <returns></returns>
        public static DateTime FirstDay(this DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }

        /// <summary>
        /// 取得日期 对应年月的 最后一天日期
        /// </summary>
        /// <returns></returns>
        public static DateTime LastDay(this DateTime datetime)
        {
            return datetime.FirstDay().AddMonths(1).AddDays(-1);
        }

        #region 根据生日计算年龄

        /// <summary>
        /// 根据生日计算年龄
        /// </summary>
        /// <param name="birthDay"></param>
        /// <returns></returns>
        public static int GetAge(this DateTime? birthDay)
        {
            if (birthDay.HasValue)
            {
                return GetAge(birthDay.Value);
            }
            return 0;
        }

        /// <summary>
        /// 根据生日计算年龄
        /// </summary>
        /// <param name="birthDay"></param>
        /// <returns></returns>
        public static int GetAge(this DateTime birthDay)
        {
            var myDate = DateTime.Now;
            var month = myDate.Month;
            var day = myDate.Day;
            int age = myDate.Year - birthDay.Year - 1;
            if (birthDay.Month < month || birthDay.Month == month && birthDay.Day <= day)
            {
                age++;
            }
            return age;
        }

        #endregion 根据生日计算年龄
    }
}