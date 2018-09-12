using System;
using System.Collections.Generic;
using System.Text;

namespace Snippets
{
    public static class LegHelper
    {
        public const string InventoryLegKeyFormat = "{0:yyyy}{0:MM}{0:dd}{1,3}{2,4}{3}{4}{5}";
        public const int InvalidAimsAtd = 9999;
        public static readonly DateTime AimsStartDate = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static short GetAimsDate(DateTime dateTime)
        {
            return (short)(dateTime - AimsStartDate).TotalDays;
        }

        public static DateTime GetDateFromAims(short day)
        {
            return AimsStartDate.AddDays(day);
        }
    }
}
