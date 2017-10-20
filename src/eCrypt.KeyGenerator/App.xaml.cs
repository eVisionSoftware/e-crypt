namespace eVision.eCrypt.KeyGenerator
{
    using System;
    using System.Windows;
    using System.Windows.Threading;
    using NLog;
    using MessageBox = Xceed.Wpf.Toolkit.MessageBox;
    using eCrypt.Common;

    public partial class App : Application
    {
        public App()
        {
            EmbeddedAssemblyResolver.Init((name) => SafeWriteLine($"Loaded assembly {name.Name}", name.Name),
                                          (name) => SafeWriteLine($"Could not find assembly {name.Name}", name.Name));
        }

        private static void SafeWriteLine(string message, string assemblyName)
        {
            if (assemblyName != nameof(NLog))
            {
                Debug(message);
            }
        }

        private static void Debug(string message)
        {
            LogManager.GetCurrentClassLogger().Debug(message);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is ApplicationException)
            {
                e.Handled = true;
                return;
            }

            try
            {
                LogManager.GetCurrentClassLogger().Error(e.Exception, $"Unhandled exception: {e.Exception}");
            }
            finally
            {
                MessageBox.Show(e.Exception.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
        }
    }
}
