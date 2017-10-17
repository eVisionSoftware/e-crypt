namespace eVision.eCrypt.Helpers.Extensions
{
    internal static class StringExtensions
    {
        public static string TrimStart(this string str, string preffix) =>
            str.StartsWith(preffix) ? str.Substring(preffix.Length) : str;

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
