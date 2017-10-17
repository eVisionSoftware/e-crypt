namespace eVision.Decryption.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CommandLine;
    using System.Reflection;
    using CommandLine.Text;

    public class Options
    {
        [Option('k', "key-path", Required = false, HelpText = "Full path to private key")]
        public string PrivateKeyPath { get; set; }

        [Option('d', "destination-path", Required = false, HelpText = "Full path to directory for decrypted package")]
        public string DestinationPath { get; set; }

        [Option('v', "verbose", DefaultValue = false, Required = false, HelpText = "Enable verbose logging")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (helpText) =>
                {
                    HelpText.DefaultParsingErrorsHandler(this, helpText);
                    helpText.Heading = "Self extractor Version " + Assembly.GetExecutingAssembly().GetName().Version;
                    helpText.Copyright = "Self extractor will decrypt encrypted data based on provided PGP private key.";
                    helpText.AddPreOptionsLine("Please read options:");
                    helpText.AddPostOptionsLine("Examples:");
                    helpText.AddPostOptionsLine("{Environment.NewLine}");
                    helpText.AddPostOptionsLine(@"
Example 1:
Extract encrypted data to provided destination folder using provided private PGP key file path.
...eVision.exe --key-path=""C:\Keys\pub-key-file.asc""--destination-path=""C:\Decrypted Package\""
");
                    helpText.AddPostOptionsLine(Environment.NewLine);
                });
        }

        public string ErrorMessage { get; private set; }
        public bool IsInvalid { get { return !string.IsNullOrEmpty(ErrorMessage); } }

        public bool IsConsoleMode
        {
            get { return !string.IsNullOrEmpty(PrivateKeyPath) && !string.IsNullOrEmpty(DestinationPath); }
        }

        public bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public bool IsDevelopment
        {
            get
            {
#if SELF_EXTRACTOR_COMPILER
                return false;
#else
                return true;
#endif
            }
        }

        public bool ShouldLogDebug
        {
            get { return Verbose || IsDebug || IsDevelopment; }
        }

        public static Options Parse(string[] args)
        {
            var options = new Options();
            var parser = new Parser(settings => { });

            if (!parser.ParseArguments(args, options))
            {
                options.ErrorMessage = "Could not parse arguments. Invalid parameters are: "
                    + string.Join(",", options.LastParserState.Errors.Select(e => e.BadOption.LongName));
                return options;
            }

            options.ErrorMessage = options.CustomValidate();
            return options;
        }

        private string CustomValidate()
        {
            var errors = new List<string>();

            if (PrivateKeyPath != null && !File.Exists(PrivateKeyPath))
            {
                errors.Add("Private key file doesn't exists: " + PrivateKeyPath);
            }

            if (DestinationPath != null && !Directory.Exists(DestinationPath))
            {
                errors.Add("Destination directory doesn't exists: " + DestinationPath);
            }

            return errors.Any() ? string.Join(Environment.NewLine, errors) : null;
        }
    }
}
