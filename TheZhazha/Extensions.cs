using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shock.Logger;

namespace TheZhazha
{
    public static class Extensions
    {
        public static void LogExceptions(this Task task)
        {
            task.ContinueWith(t =>
            {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                    LoggerFacade.Log(exception.Message);
            },
            TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
