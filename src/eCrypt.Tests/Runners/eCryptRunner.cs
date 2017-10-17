namespace eVision.eCrypt.Tests.Runners
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common;

    internal class eCryptRunner
    {
        private static readonly string FileName = @"eVision.eCrypt.exe";
        private static readonly IReadOnlyCollection<string> ToolLocations = new[]
        {
            $@".\{FileName}",
            $@"..\..\..\eCrypt\bin\Debug\{FileName}",
            $@"..\..\..\eCrypt\bin\Release\{FileName}"
        };

        public async Task<ProcessResult> RunAsync(string ouputPath, string publicKeyPath, string targetPath)
        {
            return await new ProcessRunner(FindeCrypt())
                .RunAsync(new Dictionary<string, string>()
                {
                    {"output-path", ouputPath},
                    {"key-path", publicKeyPath},
                    {"target", targetPath}
                });
        }

        public async Task<ProcessResult> RunAsyncWithPublicKey(string ouputPath, string publicKey, string targetPath)
        {
            return await new ProcessRunner(FindeCrypt())
                .RunAsync(new Dictionary<string, string>()
                {
                    {"output-path", ouputPath},
                    {"key", publicKey},
                    {"target", targetPath}
                });
        }

        private static string FindeCrypt() => ToolLocations
            .Select(loc => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, loc))
            .First(File.Exists);
    }
}
