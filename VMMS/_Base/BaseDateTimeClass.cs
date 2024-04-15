using System;

namespace VMMS
{
    /// <summary>
    /// 日期时间操作类
    /// </summary>
    public class BaseDateTimeClass
    {
        public static readonly DateTime BaseDate = Convert.ToDateTime("1900-01-01");//基准日期

        /// <summary>
        /// 简易计算日期差天数
        /// </summary>
        /// <param name="dt1">日期1</param>
        /// <param name="dt2">日期2</param>
        /// <returns>日期2-日期1的天数</returns>
        public static int DateDiff(DateTime dt1, DateTime dt2)
        {
            int diff = 0;
            DateTime d1 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt1.Year, dt1.Month, dt1.Day));
            DateTime d2 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", dt2.Year, dt2.Month, dt2.Day));
            diff = (d2 - dt1).Days;
            return diff;
        }

        /// <summary>
        /// 返回本月月初
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>DateTime</returns>
        public static DateTime GetCurrentMonthStart(DateTime dt)
        {
            return dt.AddDays(1 - dt.Day).Date;
        }

        /// <summary>
        /// 返回本月月末
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>DateTime</returns>
        public static DateTime GetCurrentMonthEnd(DateTime dt)
        {
            return (GetCurrentMonthStart(dt).AddMonths(1).AddDays(-1)).AddDays(1).AddMilliseconds(-1);
        }
    }
}
