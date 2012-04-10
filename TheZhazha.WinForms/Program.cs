using System;
using System.Windows.Forms;
using Shock.Logger;
using System.Drawing;

namespace TheZhazha.WinForms
{
    static class Program
    {
        private static NotifyIcon _icon;

        [STAThread]
        static void Main()
        {
            new FileLogger();
            new ConsoleLogger();
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var menu = new ContextMenu();
            menu.MenuItems.Add(new MenuItem("E&xit", OnExitMenuItemClick));

            _icon = new NotifyIcon();
            _icon.Icon = Properties.Resources.zhazha;
            _icon.Text = "Zhazha is watching you";
            _icon.ContextMenu = menu;
            _icon.Visible = true;

            Zhazha.Manager.StatusChanged += (s, e) =>
            {
                _icon.Text = e.Argument;
                _icon.BalloonTipText = e.Argument;
                _icon.ShowBalloonTip(3000, "TheZhazha", e.Argument, ToolTipIcon.Info);
            };

            Zhazha.Start();
            Application.Run();
        }

        private static void OnExitMenuItemClick(object sender, EventArgs e)
        {
            Zhazha.Stop();
            _icon.Visible = false;
            Application.Exit();
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
