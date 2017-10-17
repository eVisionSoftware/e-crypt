namespace eVision.eCrypt.KeyGenerator.Helpers.Extensions
{
    internal static class StringExtensions
    {
        public static string TrimSuffix(this string input, string suffixToRemove)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }

            return input;
        }
    }
}
