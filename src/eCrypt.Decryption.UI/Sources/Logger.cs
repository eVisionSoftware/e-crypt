namespace eVision.Decryption.UI
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;

    public static class Logger
    {
        private static readonly string EventLogName = "Application";
        private static readonly string CurrentExePath = Process.GetCurrentProcess().MainModule.FileName;
        private static readonly string CurrentDirectory = Path.GetDirectoryName(CurrentExePath);
        private static readonly string CurrentFilename = Path.GetFileNameWithoutExtension(CurrentExePath);
        private static readonly string LogPath = Path.Combine(CurrentDirectory, CurrentFilename + ".log");
        private static bool _logDebug;

        public static void Error(string message)
        {
            Write(message, "ERROR");
            TryWriteToEventLog(message);
        }

        public static void Debug(string message)
        {
            if (_logDebug)
            {
                Write(message, "DEBUG");
            }
        }

        public static void Debug(string format, params object[] args)
        {
            Debug(string.Format(format, args));
        }

        private static void Write(string message, string level)
        {
            try
            {
                using (var writer = File.AppendText(LogPath))
                {
                    string formatted = string.Format("[{0:dd/MM/yyyy HH:mm:ss}] {1} {2} {3}", DateTime.Now, level, message, Environment.NewLine);
                    writer.Write(formatted);
                    Console.WriteLine(formatted);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void TryWriteToEventLog(string message)
        {
            try
            {
                using (var eventLog = new EventLog(EventLogName))
                {
                    eventLog.Source = EventLogName;
                    eventLog.WriteEntry(message, EventLogEntryType.Error);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void Init(bool isVerbose, bool isConsoleMode)
        {
            _logDebug = isVerbose;
            if (isConsoleMode)
            {
                //it should be first usage of Console
                RedirectConsoleOutputToParentProcess();
            }
        }

        public static void RedirectConsoleOutputToParentProcess()
        {
            AttachConsole(ATTACH_PARENT_PROCESS);
        }

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
    }
}