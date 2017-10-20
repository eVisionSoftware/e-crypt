namespace eVision.Encryption
{
    using System;
    using System.IO;
    using Org.BouncyCastle.Bcpg;
    using Org.BouncyCastle.Bcpg.OpenPgp;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Prng;
    using Org.BouncyCastle.Security;

    /// <summary>
    /// Based on link <see cref="http://stackoverflow.com/questions/892249/problem-generating-pgp-keys" />
    /// </summary>
    public static class PgpKeyGenerator
    {
        private const int RsaKeySize = 4096;

        public static void GenerateKey(string username, string password, string outputDirectory)
        {
            string publicKeyPath = Path.Combine(outputDirectory, "pub.asc"),
                   privateKeyPath = Path.Combine(outputDirectory, "secret.asc");

            GenerateKey(username, password, publicKeyPath, privateKeyPath);
        }

        public static void GenerateKey(string username, string password, string publicKeyPath, string privateKeyPath)
        {
            AsymmetricCipherKeyPair keyPair = GenerateKeys();

            using (FileStream publicKeyStream = File.Create(publicKeyPath))
            using (FileStream privateKeyStream = File.Create(privateKeyPath))
            {
                ExportKeyPair(privateKeyStream, publicKeyStream, keyPair.Public, keyPair.Private, username, password.ToCharArray(), true);
            }
        }

        private static AsymmetricCipherKeyPair GenerateKeys()
        {
            var kpgen = new RsaKeyPairGenerator();
            kpgen.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), RsaKeySize));
            return kpgen.GenerateKeyPair();
        }

        private static void ExportKeyPair(Stream secretOut,
                                          Stream publicOut,
                                          AsymmetricKeyParameter publicKey,
                                          AsymmetricKeyParameter privateKey,
                                          string identity,
                                          char[] passPhrase,
                                          bool armor)
        {
            if (armor)
            {
                secretOut = new ArmoredOutputStream(secretOut);
            }

            var secretKey = new PgpSecretKey(PgpSignature.DefaultCertification,
                                             PublicKeyAlgorithmTag.RsaGeneral,
                                             publicKey,
                                             privateKey,
                                             DateTime.Now,
                                             identity,
                                             SymmetricKeyAlgorithmTag.Cast5,
                                             passPhrase,
                                             null,
                                             null,
                                             new SecureRandom());

            secretKey.Encode(secretOut);
            secretOut.Close();

            if (armor)
            {
                publicOut = new ArmoredOutputStream(publicOut);
            }

            PgpPublicKey key = secretKey.PublicKey;

            key.Encode(publicOut);
            publicOut.Close();
        }
    }
}
