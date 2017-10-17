namespace eVision.Encryption
{
    using System;
    using System.Linq;
    using System.IO;
    using Org.BouncyCastle.Bcpg.OpenPgp;
    using Org.BouncyCastle.Utilities.IO;

    public static class PgpDecryptor
    {
        public static void Decrypt(Stream inputStream, Stream privateKeyStream, string passPhrase, string outputFilePath)
        {
            var objectFactory = new PgpObjectFactory(PgpUtilities.GetDecoderStream(inputStream));
            var secretKeyRing = new PgpSecretKeyRingBundle(PgpUtilities.GetDecoderStream(privateKeyStream));

            PgpEncryptedDataList encryptedDataList = (objectFactory.NextPgpObject() as PgpEncryptedDataList)
                                                  ?? (PgpEncryptedDataList)objectFactory.NextPgpObject();

            PgpObjectFactory plainFact = RetrievePgpObjectFactory(encryptedDataList, secretKeyRing, passPhrase);
            PgpObject message = plainFact.NextPgpObject();

            if (message is PgpOnePassSignatureList)
            {
                throw new PgpException("Encrypted message contains a signed message - not literal data.");
            }

            if (message is PgpCompressedData)
            {
                DecryptCompressedData(message, outputFilePath);
            }
            else if (message is PgpLiteralData)
            {
                PipeStreamToFile(outputFilePath, (PgpLiteralData)message);
            }
            else
            {
                throw new PgpException("Message is not a simple encrypted file - type unknown.");
            }
        }

        private static void DecryptCompressedData(PgpObject message, string outputFile)
        {
            PgpCompressedData cData = (PgpCompressedData)message;
            PgpObjectFactory objectFactory;

            using (Stream compDataIn = cData.GetDataStream())
            {
                objectFactory = new PgpObjectFactory(compDataIn);
            }

            message = objectFactory.NextPgpObject();
            if (message is PgpOnePassSignatureList)
            {
                message = objectFactory.NextPgpObject();
            }

            PipeStreamToFile(outputFile, (PgpLiteralData)message);
        }

        private static PgpObjectFactory RetrievePgpObjectFactory(PgpEncryptedDataList dataList, PgpSecretKeyRingBundle secretKeyRing, string passPhrase)
        {
            PgpPublicKeyEncryptedData publicKeyEncryptedData = dataList.GetEncryptedDataObjects().Cast<PgpPublicKeyEncryptedData>()
                .FirstOrDefault(dd => FindSecretKeyByKeyId(secretKeyRing, dd.KeyId, passPhrase.ToCharArray()) != null);

            if (publicKeyEncryptedData == null)
            {
                throw new ArgumentException("Secret key for message not found.");
            }

            PgpPrivateKey privateKey = FindSecretKeyByKeyId(secretKeyRing, publicKeyEncryptedData.KeyId, passPhrase.ToCharArray());
            using (Stream stream = publicKeyEncryptedData.GetDataStream(privateKey))
            {
                return new PgpObjectFactory(stream);
            }
        }

        private static void PipeStreamToFile(string outputFilePath, PgpLiteralData literalData)
        {
            using (Stream fileStream = File.Create(outputFilePath))
            {
                Stream decryptedStream = literalData.GetInputStream();
                Streams.PipeAll(decryptedStream, fileStream);
            }
        }

        private static PgpPrivateKey FindSecretKeyByKeyId(PgpSecretKeyRingBundle secretKeyRing, long keyId, char[] passPhrase)
        {
            PgpSecretKey pgpSecKey = secretKeyRing.GetSecretKey(keyId);
            return pgpSecKey == null ? null : pgpSecKey.ExtractPrivateKey(passPhrase);
        }
    }
}
