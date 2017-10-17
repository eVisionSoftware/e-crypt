namespace eVision.Decryption.UI
{
    using System.IO;

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

        public static string TrimStart(this string str, string preffix)
        {
            return str.StartsWith(preffix) ? str.Substring(preffix.Length) : str;
        }

        public static string TrimEnd(this string input, string suffix)
        {
            if (input == null || suffix == null)
            {
                return input;
            }

            return input.EndsWith(suffix) ? input.Substring(0, input.Length - suffix.Length) : input;
        }
    }
}
