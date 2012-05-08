using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheZhazha.Models
{
    public static class Diablo3
    {
        private static readonly DateTime _releaseTime = new DateTime(2012, 5, 15);

        public static TimeSpan GetTimeLeft()
        {
            return _releaseTime.Subtract(DateTime.Now);
        }

        public static string GetTimeLeftStr()
        {
            var delta = GetTimeLeft();
            return string.Format(
                "Evil is Back in{4}{0} days, {1} hours, {2} minutes and {3} seconds!",
                delta.Days,
                delta.Hours,
                delta.Minutes,
                delta.Seconds,
                Environment.NewLine);
        }
    }
}
