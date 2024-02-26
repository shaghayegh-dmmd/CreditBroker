using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CreditBrokerMvc.Helper
{
    public static class HelperInfra
    {
       
        public static string EnglishNumbersToPersian(this string str)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrEmpty(str)) return str;
            return
                str.Replace("0", "۰")
                    .Replace("1", "۱")
                    .Replace("2", "۲")
                    .Replace("3", "۳")
                    .Replace("4", "۴")
                    .Replace("5", "۵")
                    .Replace("6", "۶")
                    .Replace("7", "۷")
                    .Replace("8", "۸")
                    .Replace("9", "۹");
        }
        public static string GetJalaliFromDateTimeGregorian(System.DateTime gerigorianDate)
        {
            var cal = new PersianCalendar();
            try
            {
                var month = cal.GetMonth(gerigorianDate).ToString().PadLeft(2, '0');

                var day = cal.GetDayOfMonth(gerigorianDate).ToString().PadLeft(2, '0');

                return cal.GetYear(gerigorianDate) + "/" + month + "/" + day;
            }
            catch
            {
                return "";
            }
        }
    }
}