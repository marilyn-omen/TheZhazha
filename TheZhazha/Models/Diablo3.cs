using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheZhazha.Models
{
    public static class Diablo3
    {
        private static readonly DateTime _releaseTime = new DateTime(2012, 5, 15, 1, 0, 0);
        private static readonly DateTime _releaseTime2 = new DateTime(2012, 6, 7, 1, 0, 0);
        
        public static TimeSpan GetTimeLeft()
        {
            return _releaseTime.Subtract(DateTime.Now);
        }
        
        public static TimeSpan GetTimeLeft2()
        {
            return _releaseTime2.Subtract(DateTime.Now);
        }

        public static string GetTimeLeftStr()
        {
            var delta = GetTimeLeft();
            var delta2 = GetTimeLeft2();
            if (delta2.Ticks < 0)
            {
                return string.Format(
                    "Evil is already Back for all!!!{0}",
                    Environment.NewLine);
            }

            if (delta.Ticks < 0)
            {
                return string.Format(
                "Evil is already back, but some russian speaking horses will wait{4}{0} days, {1} hours, {2} minutes and {3} seconds!",
                delta2.Days,
                delta2.Hours,
                delta2.Minutes,
                delta2.Seconds,
                Environment.NewLine);
            }

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
