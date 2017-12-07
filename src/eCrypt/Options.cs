namespace eVision.eCrypt
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using CommandLine;
    using CommandLine.Text;

    internal class Options
    {
        [Option('o', "output-path", Required = false, DefaultValue = "./eVision.exe", HelpText = "Full path of generated executable including exe file extension")]
        public string OutputAssemblyPath { get; set; }

        [Option('k', "key-path", Required = false, HelpText = "Path to file containing public key for encryption")]
        public string PublicKeyPath { get; set; }

        [Option("key", Required = false, HelpText = "Public key for encryption")]
        public string PublicKey { get; set; }

        [Option('t', "target", Required = true, HelpText = "Folder of file that should be encrypted and embedded into generated executable")]
        public string EncryptionTargetPath { get; set; }

        [Option('v', "content-version", Required = false, HelpText = "Version of the content to be encrypted. If provided it will be used for 'file version' property of generated executable")]
        public string ContentVersion { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        public static Options TryParse(string[] args, TextWriter errorWriter)
        {
            var options = new Options();
            var parser = new Parser(settings =>
            {
                settings.HelpWriter = errorWriter;
            });

            if (!parser.ParseArguments(args, options))
            {
                return null;
            }

            var customError = options.CustomValidate();

            if (customError != null)
            {
                errorWriter.WriteLine(customError);
                return null;
            }

            ExtractKeyFromFile(options);

            return options;
        }

        private static void ExtractKeyFromFile(Options options)
        {
            if (options.PublicKeyPath != null)
            {
                options.PublicKey = File.ReadAllText(options.PublicKeyPath);
            }
        }

        private string CustomValidate()
        {
            var errors = new List<string>();

            if (PublicKeyPath != null && !File.Exists(PublicKeyPath))
            {
                errors.Add($"Public key file '{PublicKeyPath}' doesn't exists");
            }

            if (PublicKeyPath == null && PublicKey == null)
            {
                errors.Add("Public key should be specified in key or key-path parameter");
            }

            if (PublicKeyPath != null && PublicKey != null)
            {
                errors.Add("Source of the public key should be specified in key or key-path parameter");
            }

            if (EncryptionTargetPath != null && !PathExists(EncryptionTargetPath))
            {
                errors.Add($"Target '{EncryptionTargetPath}' doesn't exists");
            }

            if (OutputAssemblyPath != null)
            {
                if (!Directory.Exists(Path.GetDirectoryName(OutputAssemblyPath) ?? ""))
                {
                    errors.Add($"Output directory '{OutputAssemblyPath}' doesn't exist");
                }

                if (!OutputAssemblyPath.ToLowerInvariant().EndsWith(".exe"))
                {
                    errors.Add($"Output assembly path `{OutputAssemblyPath}` should point to executable with extension 'exe'");
                }
            }

            if (ContentVersion != null && !Regex.IsMatch(ContentVersion, @"^(\d\.){3}\d$"))
            {
                errors.Add("Content version parameter should be in '1.1.1.1' format");
            }

            return errors.Any() ? string.Join(Environment.NewLine, errors) : null;
        }

        private bool PathExists(string path) => File.Exists(path) || Directory.Exists(path);

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage() => HelpText.AutoBuild(this,
            (helpText) =>
            {
                HelpText.DefaultParsingErrorsHandler(this, helpText);
                helpText.Heading = $"eCrypt Version {Assembly.GetExecutingAssembly().GetName().Version}";
                helpText.Copyright =
                    @"eCrypt will generate self extractable executable that contains encrypted with public PGP key target file or directory. Read user manual for usage instructions.";
                helpText.AddPreOptionsLine("Please read options:");
                helpText.AddPostOptionsLine("Examples:");
                helpText.AddPostOptionsLine($"{Environment.NewLine}");
                helpText.AddPostOptionsLine(@"
Example 1:
Generate self extractable executable including encrypted target directory using public PGP key file.
... --output-path=C:\Destination\eVision.exe --key-path=""C:\Keys\public key.asc"" --target=C:\SourcePackage\
Example 2:
Generate self extractable executable including encrypted target file using public PGP key file.
... --output-path=C:\Destination\eVision.exe --key-path=""C:\Keys\public key.asc"" --target=C:\SourcePackage\package-1.0.0.zip
Example 3:
Generate self extractable executable and providing content version.
... --output-path=C:\Destination\eVision.exe --key-path=""C:\Keys\public key.asc"" --target=C:\SourcePackage\package-1.0.0.zip --content-version=""7.7.7.7""
");
                helpText.AddPostOptionsLine($"{Environment.NewLine}");
            });

        public static Options FromString(string arguments)
        {
            var parsed = new Options();
            string[] args = Regex.Split(arguments, @"\s(?=-)")
                .Select(a => a.Trim(' '))
                .Select(a => a.Replace("\"", ""))
                .ToArray();

            if (!new Parser().ParseArguments(args, parsed))
            {
                throw new FormatException($"Command line arguments not formatted correctly: {arguments}");
            }

            return parsed;
        }
    }
}
