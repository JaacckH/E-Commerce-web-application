using System;
using System.Collections.Generic;
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

    }
}
