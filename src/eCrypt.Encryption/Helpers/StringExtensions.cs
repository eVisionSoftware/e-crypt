using System.IO;

namespace eVision.Encryption.Helpers
{
    internal static class StringExtensions
    {
        public static Stream OpenStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
