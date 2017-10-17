namespace eVision.eCrypt.Helpers.Extensions
{
    using System.IO;
    using System.Reflection;

    internal static class AssemblyExtensions
    {
        public static string GetStringResource(this Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
