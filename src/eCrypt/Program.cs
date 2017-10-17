namespace eVision.eCrypt
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using Transformations;
    using Common;
    using static System.IO.Path;

    internal class Program
    {
        private static readonly TextWriter Logger = Console.Out;

        internal static int Main(string[] args)
        {
            string oldDirectory = Directory.GetCurrentDirectory();
            try
            {
                AdjustCurrentDirectory();
                EmbeddedAssemblyResolver.Init((name) => Logger.WriteLine("Loaded assembly {0}", name.Name),
                                              (name) => Logger.WriteLine("Could not find assembly {0}", name.Name));
                Logger.WriteLine("Started process as {0}", Environment.Is64BitProcess ? "x64" : "x86");
                Options options = Options.TryParse(args, Logger);
                if (options == null)
                {
                    return (int)ProcessCode.InvalidArguments;
                }
                
                string targetFilePath = options.EncryptionTargetPath;
                bool useArchive = IsDirectory(options.EncryptionTargetPath);

                using (var tempFiles = new TempFileCollection())
                {
                    if (useArchive)
                    {
                        targetFilePath = Combine(AppDomain.CurrentDomain.BaseDirectory, $"{GetRandomFileName()}.zip");
                        tempFiles.AddFile(targetFilePath, false);
                        ZipFile.CreateFromDirectory(options.EncryptionTargetPath, targetFilePath, CompressionLevel.NoCompression, false);
                    }

                    using (var compiler = new SelfExtractorCompiler(new EncryptFileTransformation(options.PublicKey))
                        .IncludingFiles(targetFilePath))
                    {
                        string path = compiler.Compile(options.OutputAssemblyPath);

                        Logger.WriteLine($"Self extractable executable was generated at {path} using key {options.PublicKey}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex);
                return (int)ProcessCode.Failed;
            }
            finally
            {
                Directory.SetCurrentDirectory(oldDirectory);
            }
            return (int)ProcessCode.Success;
        }

        private static void AdjustCurrentDirectory()
        {
            string directory = GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Logger.WriteLine($"Using {directory} as current directory");
            Directory.SetCurrentDirectory(directory);
        }

        private static bool IsDirectory(string path) => File.GetAttributes(path).HasFlag(FileAttributes.Directory);
    }
}
