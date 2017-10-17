namespace eVision.eCrypt.Common
{
    public static class PGPValidator
    {
        public static bool IsPrivateKey(string key)
        {
            return !string.IsNullOrEmpty(key)
                && key.Contains("BEGIN PGP PRIVATE KEY BLOCK");
        }

        public static bool IsPublicKey(string key)
        {
            return !string.IsNullOrEmpty(key)
                && key.Contains("BEGIN PGP PUBLIC KEY BLOCK");
        }
    }
}
