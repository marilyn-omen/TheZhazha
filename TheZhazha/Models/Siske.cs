using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheZhazha.Utils;

namespace TheZhazha.Models
{
    public class Siske
    {
        private const string _url = "http://sizedoesmatter.tumblr.com/random";
        private const int _tries = 3;
        private static readonly Regex _regex =
            new Regex(
                "<div\\sclass=\"photo-panel\">\\s*<a href=\"[^\"]+\"><img\\ssrc=\"(?<url>[^\"]+)\"",
                RegexOptions.IgnoreCase);
        private static bool _isLoading;

        private static readonly string[] _smilies = new[]
                                                        {
                                                            "(inlove)",
                                                            ":$",
                                                            ":P",
                                                            ":*",
                                                            "(wasntme)",
                                                            "(angel)",
                                                            "(mm)",
                                                            "(smirk)",
                                                            "(happy)",
                                                            "(h)"
                                                        };

        public static void Load(Action<string> callback)
        {
            if (callback == null || _isLoading)
                return;

            Task.Factory.StartNew(
                () =>
                    {
                        _isLoading = true;
                        string response;
                        var n = 0;
                        do
                        {
                            response = DoLoad();
                            n++;
                        } while (string.IsNullOrEmpty(response) && n < _tries);
                        if(string.IsNullOrEmpty(response))
                        {
                            response = "Сиське не обнаружены. Три раза пыталась, бля буду!";
                        }
                        callback(response);
                        _isLoading = false;
                    }
                ).LogExceptions();
        }

        private static string DoLoad()
        {
            var page = WebUtils.GetPageHtml(_url);
            var match = _regex.Match(page);
            if (match.Success)
            {
                var n = Zhazha.Rnd.Next(_smilies.Length);
                return string.Format("{0} {1}", _smilies[n], match.Groups["url"].Value);
            }
            return null;
        }
    }
}
