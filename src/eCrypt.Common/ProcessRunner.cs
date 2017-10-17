namespace eVision.eCrypt.Common
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using System.Threading.Tasks;

    internal class ProcessRunner
    {
        private readonly List<string> output = new List<string>();
        private readonly string path;
        private bool adjustDirectory;

        public ProcessRunner(string exeFilePath)
        {
            path = exeFilePath;
        }

        public async Task<ProcessResult> RunAsync(Dictionary<string, string> arguments)
        {
            string directory = adjustDirectory ? Path.GetDirectoryName(path) : Directory.GetCurrentDirectory();
            string args = string.Join(" ", arguments.Select(a => string.Format("--{0}{1}", a.Key, FormatValue(a.Value))));

            var startInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = path,
                Arguments = args
            };

            return await UsingCurrentDir(directory, async () =>
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.OutputDataReceived += (s, e) => output.Add(e.Data);
                    process.ErrorDataReceived += (s, e) => output.Add(e.Data);
                    process.BeginOutputReadLine();
                    int code = await process.WaitForExitAsync();

                    return new ProcessResult((ProcessCode)code, string.Join(Environment.NewLine, output), path, args);
                }
            });
        }

        private static string FormatValue(string value)
        {
            return string.IsNullOrEmpty(value) ? "" : string.Format(value.Contains(" ") ? "=\"{0}\"" : "={0}", value);
        }

        public ProcessRunner AdjustCurrentDirectory()
        {
            adjustDirectory = true;
            return this;
        }

        private T UsingCurrentDir<T>(string currentDir, Func<T> action)
        {
            string oldCurrentDir = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(currentDir);
                return action();
            }
            finally
            {
                Directory.SetCurrentDirectory(oldCurrentDir);
            }
        }
    }

    internal class ProcessResult
    {
        public ProcessResult(ProcessCode code, string output, string executablePath, string arguments)
        {
            Code = code;
            Output = output;
            ExecutablePath = executablePath;
            Arguments = arguments;
        }

        public string ExecutablePath { get; private set; }
        public string Arguments { get; private set; }
        public string Output { get; private set; }
        public ProcessCode Code { get; private set; }

        public bool IsSuccess
        {
            get { return Code == ProcessCode.Success; }
        }
    }

    internal enum ProcessCode
    {
        Success = 0,
        Failed = 2,
        InvalidArguments = 3
    }
}
