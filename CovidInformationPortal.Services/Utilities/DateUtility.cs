using System;
using System.Collections.Generic;
using System.Text;

namespace CovidInformationPortal.Services.Utilities
{
    public static class DateUtility
    {
        private static Dictionary<string, int> months = new Dictionary<string, int>
        {
            { "януари", 1},
            { "февруари", 2},
            { "март", 3},
            { "април", 4},
            { "май", 5},
            { "юни", 6},
            { "юли", 7},
            { "август", 8},
            { "септември", 9},
            { "октомври", 10},
            { "ноември", 11},
            { "декември", 12},
        };

        public static DateTime GetDateByString(string value)
        {
            var dateString = value.Trim().Split(" ");
            var month = months[dateString[1].ToLower()];
            var date = new DateTime(int.Parse(dateString[2]), month, int.Parse(dateString[0]));

            return date;
        }
    }
}
