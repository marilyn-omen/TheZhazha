using System;
using System.Windows.Forms;
using Shock.Logger;

namespace TheZhazha.WinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            new FileLogger();
            new ConsoleLogger();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }

        private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            string logMessage;
            string userMessage = logMessage = "Unknown exception.";
            if (exception != null)
            {
                userMessage = logMessage = exception.Message;
                if ((exception.InnerException != null) && !String.IsNullOrEmpty(exception.InnerException.Message))
                {
                    logMessage += String.Format("\n\tInner exception: {0}", exception.InnerException.Message);
                }
                logMessage += String.Format("\n\tStack trace: {0}", exception.StackTrace);
            }

            LoggerFacade.Log(logMessage, Importance.Error);
            MessageBox.Show(
                String.Format("An error occured:{0}{0}{1}{0}{0}Check log file for details.", Environment.NewLine, userMessage),
                @"Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            Environment.Exit(1);
        }
    }
}
