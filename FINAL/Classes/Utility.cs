using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class Utility
    {
        public static String getDateFromDay(int day)
        {
            DateTime date = new DateTime(DateTime.Now.Year, 1, 1).AddDays(day - 1);
            return date.Date.ToString().Replace(" 00:00:00", "");
        }

        public static String formatPrice(String price)
        {
            if (String.IsNullOrEmpty(price))
            {
                return price;
            }
            return int.Parse(price).ToString("N", new CultureInfo("en-US")).Replace(".00", "");
        }

    }
}
