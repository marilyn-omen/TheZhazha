using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheZhazha.Utils;

namespace TheZhazha.Models
{
    public static class GameNews
    {
        private const int _maxNewCount = 5;
        private const string _url = "http://gameplay.com.ua/tracker";
        private const string _baseUrl = "http://gameplay.com.ua";
        private static readonly Regex _regex = new Regex("<tr\\sclass=\"(?:odd|even)\"><td>[\\w\\s]+</td><td><a href=\"(?<url>[\\w/]+)\">(?<title>[^<]+)</a>", RegexOptions.IgnoreCase);
        private static bool _isLoading;

        public static void Load(Action<string> callback)
        {
            if (callback == null || _isLoading)
                return;

            Task.Factory.StartNew(
                () =>
                    {
                        _isLoading = true;
                        var sb = new StringBuilder();
                        var page = WebUtils.GetPageHtml(_url);
                        var matches = _regex.Matches(page);
                        if (matches.Count > 0)
                        {
                            for (var i = 0; i < matches.Count && i < _maxNewCount; i++)
                            {
                                if (i > 0)
                                    sb.AppendLine();
                                sb.Append(i + 1);
                                sb.Append(". ");
                                sb.AppendLine(matches[i].Groups["title"].Value);
                                sb.Append("    [ ");
                                sb.Append(_baseUrl + matches[i].Groups["url"].Value);
                                sb.Append(" ]");
                            }
                        }
                        _isLoading = false;
                        callback(sb.ToString());
                    }
                );
        }
    }
}
