namespace eVision.eCrypt.Transformations
{
    using Encryption;

    public class EncryptFileTransformation : IFileTransformation
    {
        private const string EncryptedExtensions = "pgp";
        private readonly string _publicKey;

        public EncryptFileTransformation(string publicKey)
        {
            _publicKey = publicKey;
        }

        public string TransformToNewFile(string sourceFilePath)
        {
            string outputPath = $"{sourceFilePath}.{EncryptedExtensions}";
            PgpEncryptor.EncryptFile(sourceFilePath, outputPath, _publicKey);
            return outputPath;
        }
    }
}
