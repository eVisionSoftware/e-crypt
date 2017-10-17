namespace eVision.Encryption.Tests.Utils
{
    using System.CodeDom.Compiler;
    using static System.IO.Path;

    public static class TempFileCollectionExtensions
    {
        public static string AddFileWithExtension(this TempFileCollection files, string extension) =>
            AddFile(files, $"{GetRandomFileName()}.{extension}");

        public static string AddFile(this TempFileCollection files, string fileName = null, bool keepLife = false)
        {
            string file = fileName ?? GetRandomFileName(),
                   directory = string.IsNullOrEmpty(files.TempDir) ? GetTempPath() : files.TempDir,
                   fullPath = Combine(directory, file);

            files.AddFile(fullPath, keepLife);
            return fullPath;
        }
    }
}
