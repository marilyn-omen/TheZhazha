using System;

namespace TheZhazha
{
    public class Quot
    {
        // delay in minutes
        public const int QuotDelayPerUser = 5;
        public const string Suffix1 = "(c)";
        public const string Suffix2 = "(с)";

        public string Text { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }

        public Quot(string text, string user)
        {
            Text = text;
            User = user;
            Date = DateTime.Now;
        }

        public static string GetQuotContent(string quot)
        {
            var idx = quot.LastIndexOf(Suffix1);
            if (idx == -1)
                idx = quot.LastIndexOf(Suffix2);
            var content = quot.Substring(0, idx);
            content = content.Replace(Environment.NewLine, " ");
            return content.Trim();
        }

        public static bool IsQuotString(string quot)
        {
            var trimmed = quot.Trim();
            return (trimmed.EndsWith(Suffix1) || trimmed.EndsWith(Suffix2));
        }
    }
}