namespace eVision.Encryption.Helpers
{
    using System;
    using System.IO;
    using System.Collections.Generic;

    public class TempDirectory : IDisposable
    {
        private readonly bool _keepDirectory;
        public string Path { get; private set; }

        public TempDirectory(string path = null, bool keepDirectory = false)
        {
            Path = path ?? System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            _keepDirectory = keepDirectory;

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
        }

        public static TempDirectory InBaseDirectory()
        {
            return new TempDirectory(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.GetRandomFileName()));
        }

        public IEnumerable<string> Files
        {
            get { return Directory.EnumerateFiles(Path, "*.*", SearchOption.AllDirectories); }
        }

        public string AddFile(string fileName = null)
        {
            return System.IO.Path.Combine(Path, fileName ?? System.IO.Path.GetRandomFileName());
        }

        public string CreateTextFile(string fileName, string content)
        {
            string path = System.IO.Path.Combine(Path, fileName);
            File.WriteAllText(path, content);
            return path;
        }

        public void Dispose()
        {
            if (!_keepDirectory)
            {
                Directory.Delete(Path, true);
            }
        }
    }
}

