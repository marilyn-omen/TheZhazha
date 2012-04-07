using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shock.Logger;

namespace TheZhazha.Models
{
    public static class QuotesLoader
    {
        public static List<QuoteGenerator> GetGeneratorsList(string path)
        {
            var result = new List<QuoteGenerator>();
            if (Directory.Exists(path))
            {
                var di = new DirectoryInfo(path);
                result.AddRange(GetGeneratorsListRecursive(di));
            }
            result.Sort();
            return result;
        }

        private static IEnumerable<QuoteGenerator> GetGeneratorsListRecursive(DirectoryInfo dir)
        {
            var result = new List<QuoteGenerator>();
            if(dir != null)
            {
                FileInfo[] files = null;
                try
                {
                    files = dir.GetFiles("*.xml");
                }
                catch (Exception ex)
                {
                    LoggerFacade.Log(ex.Message, Importance.Warning);
                }
                if (files != null)
                {
                    result.AddRange(files.Select(file => new QuoteGenerator(file.FullName)));
                }
                DirectoryInfo[] subDirs = null;
                try
                {
                    subDirs = dir.GetDirectories();
                }
                catch (Exception ex)
                {
                    LoggerFacade.Log(ex.Message, Importance.Error);
                }
                if (subDirs != null)
                {
                    foreach (var subDir in subDirs)
                    {
                        result.AddRange(GetGeneratorsListRecursive(subDir));
                    }
                }
            }
            return result;
        }
    }
}