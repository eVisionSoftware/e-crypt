namespace eVision.Decryption.UI
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Reflection;
    using eCrypt.Common;

    static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                Init();
                Options options = Options.Parse(args);
                var service = new AppService(options);
                Logger.Init(options.ShouldLogDebug, options.IsConsoleMode);

                if (options.IsInvalid)
                {
                    Logger.Error(options.ErrorMessage);
                    return (int)ProcessCode.InvalidArguments;
                }

                Logger.Debug("Started process as {0}", Environment.Is64BitProcess ? "x64" : "x86");
                Logger.Debug("Initialized{0} logger {1}{2}", options.Verbose ? " verbose" : "", options.IsDebug ? "in debug mode" : "", options.IsDevelopment ? " and compiled by Visual Studio" : " and compiled by Self Extractor Compiler");
                Logger.Debug("Embedded resources: {0}", string.Join(",", Assembly.GetExecutingAssembly().GetManifestResourceNames()));

                if (options.IsConsoleMode)
                {
                    Logger.Debug("Extracting with predefined key {0} to {1}", options.PrivateKeyPath, options.DestinationPath);
                    service.Extract(File.ReadAllText(options.PrivateKeyPath), options.DestinationPath);
                }
                else
                {
                    Logger.Debug("Starting user interface");
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm(service));
                }

                return (int)ProcessCode.Success;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return (int)ProcessCode.Failed;
            }
        }

        private static void Init()
        {
            EmbeddedAssemblyResolver.Init((name) => Logger.Debug("Loaded assembly {0}", name.Name),
                (name) => Logger.Debug("Could not find assembly {0}", name.Name));
            AppDomain.CurrentDomain.UnhandledException += (s, a) => Logger.Error("Unhandled Exception: " + a.ExceptionObject.ToString()); //TODO Application.ThreadException += (s, a) => LogException(a.Exception);
        }
    }
}
