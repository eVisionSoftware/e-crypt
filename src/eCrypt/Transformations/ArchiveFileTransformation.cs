namespace eVision.eCrypt.Transformations
{
    using System.IO;
    using System.IO.Compression;

    internal class ArchiveFileTransformation : IFileTransformation
    {
        public string TransformToNewFile(string sourceFilePath)
        {
            string destinationPath = $"{sourceFilePath}.gz";

            using (Stream file = File.OpenRead(sourceFilePath))
            {
                byte[] buffer = new byte[file.Length];

                if (file.Length != file.Read(buffer, 0, buffer.Length))
                {
                    throw new IOException($"Unable to read {sourceFilePath}");
                }

                using (Stream gzFile = File.Create(destinationPath))
                {
                    using (Stream gzip = new GZipStream(gzFile, CompressionMode.Compress))
                    {
                        gzip.Write(buffer, 0, buffer.Length);
                    }
                }
            }

            return destinationPath;
        }
    }
}
