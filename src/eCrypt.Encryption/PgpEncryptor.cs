namespace eVision.Encryption
{
    using System;
    using System.Linq;
    using System.IO;
    using Org.BouncyCastle.Bcpg;
    using Org.BouncyCastle.Bcpg.OpenPgp;
    using Org.BouncyCastle.Security;
    using Helpers;

    /// <summary>
    /// Based on link <see cref="http://stackoverflow.com/questions/10209291/pgp-encrypt-and-decrypt" />
    /// </summary>
    public static class PgpEncryptor
    {
        private const int BufferSize = 0x10000; // should always be power of 2
        private const int EncryptionBufferSize = 1 << 16;

        #region Encrypt
        /// <summary>
        /// Encrypt file
        /// </summary>
        /// <param name="armor">true if want to convert using binary-to-text encoding ASCII-Armor converter.
        /// It can be usefull in case of sending content throug http.</param>
        /// <param name="withIntegrityCheck">withIntegrityPacket - true if an integrity packet is to be included, false otherwise.
        /// Determine whether or not the resulting encrypted data will be protected using an integrity packet.</param>
        public static void EncryptFile(string inputFile, string outputFile, string publicKey, bool armor = false, bool withIntegrityCheck = false)
        {
            using (Stream publicKeyStream = publicKey.OpenStream())
            {
                PgpPublicKey pgpPublicKey = ReadPublicKey(publicKeyStream);

                var encryptedGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5,
                    withIntegrityCheck, new SecureRandom());

                encryptedGenerator.AddMethod(pgpPublicKey);

                var compressedGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);


                using (Stream outputStream = File.Create(outputFile))
                {
                    if (armor)
                    {
                        using (ArmoredOutputStream armoredStream = new ArmoredOutputStream(outputStream))
                        {
                            using (Stream encryptedOut = encryptedGenerator.Open(armoredStream, new byte[EncryptionBufferSize]))
                            {

                                PgpUtilities.WriteFileToLiteralData(compressedGenerator.Open(encryptedOut),
                                    PgpLiteralData.Binary, new FileInfo(inputFile), new byte[EncryptionBufferSize]);
                                compressedGenerator.Close();
                            }
                        }
                    }
                    else
                    {
                        using (Stream encryptedOut = encryptedGenerator.Open(outputStream, new byte[EncryptionBufferSize]))
                        {
                            PgpUtilities.WriteFileToLiteralData(compressedGenerator.Open(encryptedOut),
                                PgpLiteralData.Binary, new FileInfo(inputFile), new byte[EncryptionBufferSize]);
                            compressedGenerator.Close();
                        }
                    }
                }
              

            }
        }

        #endregion Encrypt

        #region Encrypt and Sign

        /// <summary>
        /// Encrypt and sign the file pointed to by unencryptedFileInfo
        /// </summary>
        public static void EncryptAndSign(string inputFile, string outputFile, string publicKeyFile, string privateKeyFile, string passPhrase, bool armor)
        {
            var encryptionKeys = new PgpEncryptionKeys(publicKeyFile, privateKeyFile, passPhrase);

            Contract.Requires<FileNotFoundException>(File.Exists(inputFile), "Input file [" + inputFile + "] does not exist.");
            Contract.Requires<FileNotFoundException>(File.Exists(publicKeyFile), "Public Key file [" + publicKeyFile + "] does not exist.");
            Contract.Requires<FileNotFoundException>(File.Exists(privateKeyFile), "Private Key file [" + privateKeyFile + "] does not exist.");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(passPhrase), "Invalid Pass Phrase.");

            using (Stream outputStream = File.Create(outputFile))
            {
                if (armor)
                {
                    using (ArmoredOutputStream armoredOutputStream = new ArmoredOutputStream(outputStream))
                    {
                        OutputEncrypted(inputFile, armoredOutputStream, encryptionKeys);
                    }
                } else {
                    OutputEncrypted(inputFile, outputStream, encryptionKeys);
                }
            }
        }

        private static void OutputEncrypted(string inputFile, Stream outputStream, PgpEncryptionKeys encryptionKeys)
        {
            using (Stream encryptedOut = ChainEncryptedOut(outputStream, encryptionKeys))
            {
                var unencryptedFileInfo = new FileInfo(inputFile);
                using (Stream compressedOut = ChainCompressedOut(encryptedOut))
                {
                    PgpSignatureGenerator signatureGenerator = InitSignatureGenerator(compressedOut, encryptionKeys);
                    using (Stream literalOut = ChainLiteralOut(compressedOut, unencryptedFileInfo))
                    {
                        using (FileStream inputFileStream = unencryptedFileInfo.OpenRead())
                        {
                            WriteOutputAndSign(compressedOut, literalOut, inputFileStream, signatureGenerator);
                            inputFileStream.Close();
                        }
                    }
                }
            }
        }

        private static void WriteOutputAndSign(Stream compressedOut, Stream literalOut, FileStream inputFile, PgpSignatureGenerator signatureGenerator)
        {
            int length = 0;
            byte[] buf = new byte[BufferSize];
            while ((length = inputFile.Read(buf, 0, buf.Length)) > 0)
            {
                literalOut.Write(buf, 0, length);
                signatureGenerator.Update(buf, 0, length);
            }
            signatureGenerator.Generate().Encode(compressedOut);
        }

        private static Stream ChainEncryptedOut(Stream outputStream, PgpEncryptionKeys m_encryptionKeys)
        {
            var encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.TripleDes, new SecureRandom());
            encryptedDataGenerator.AddMethod(m_encryptionKeys.PublicKey);
            return encryptedDataGenerator.Open(outputStream, new byte[BufferSize]);
        }

        private static Stream ChainCompressedOut(Stream encryptedOut)
        {
            return new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip).Open(encryptedOut);
        }

        private static Stream ChainLiteralOut(Stream compressedOut, FileInfo file)
        {
            return new PgpLiteralDataGenerator().Open(compressedOut, PgpLiteralData.Binary, file);
        }

        private static PgpSignatureGenerator InitSignatureGenerator(Stream compressedOut, PgpEncryptionKeys encryptionKeys)
        {
            const bool isCritical = false;
            const bool isNested = false;

            PublicKeyAlgorithmTag tag = encryptionKeys.SecretKey.PublicKey.Algorithm;
            var pgpSignatureGenerator = new PgpSignatureGenerator(tag, HashAlgorithmTag.Sha1);
            pgpSignatureGenerator.InitSign(PgpSignature.BinaryDocument, encryptionKeys.PrivateKey);

            string firstUserId = encryptionKeys.SecretKey.PublicKey.GetUserIds().Cast<string>().First();
            PgpSignatureSubpacketGenerator subPacketGenerator = new PgpSignatureSubpacketGenerator();
            subPacketGenerator.SetSignerUserId(isCritical, firstUserId);
            pgpSignatureGenerator.SetHashedSubpackets(subPacketGenerator.Generate());

            pgpSignatureGenerator.GenerateOnePassVersion(isNested).Encode(compressedOut);
            return pgpSignatureGenerator;
        }

        #endregion Encrypt and Sign
        
        #region Private helpers

        /// <summary>
        /// A simple routine that opens a key ring file and loads the first available key suitable for encryption.
        /// </summary>
        private static PgpPublicKey ReadPublicKey(Stream inputStream)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);
            PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(inputStream);

            // we just loop through the collection till we find a key suitable for encryption, in the real
            // world you would probably want to be a bit smarter about this.
            // iterate through the key rings.
            foreach (PgpPublicKeyRing keyRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey publicKey in keyRing.GetPublicKeys())
                {
                    if (publicKey.IsEncryptionKey)
                    {
                        return publicKey;
                    }
                }
            }

            throw new ArgumentException("Can't find encryption key in key ring.");
        }

        #endregion Private helpers

        public class PgpEncryptionKeys
        {
            public PgpPublicKey PublicKey { get; private set; }

            public PgpPrivateKey PrivateKey { get; private set; }

            public PgpSecretKey SecretKey { get; private set; }

            /// <summary>
            /// Initializes a new instance of the EncryptionKeys class.
            /// Two keys are required to encrypt and sign data. Your private key and the recipients public key.
            /// The data is encrypted with the recipients public key and signed with your private key.
            /// </summary>
            /// <param name="publicKeyPath">The key used to encrypt the data</param>
            /// <param name="privateKeyPath">The key used to sign the data.</param>
            /// <param name="passPhrase">The (your) password required to access the private key</param>
            /// <exception cref="ArgumentException">Public key not found. Private key not found. Missing password</exception>
            public PgpEncryptionKeys(string publicKeyPath, string privateKeyPath, string passPhrase)
            {
                Contract.Requires<ArgumentException>(File.Exists(publicKeyPath), "Public key file not found", "publicKeyPath");
                Contract.Requires<ArgumentException>(File.Exists(privateKeyPath), "Private key file not found", "privateKeyPath");
                Contract.Requires<ArgumentException>(File.Exists(passPhrase), "passPhrase is null or empty.", "passPhrase");

                PublicKey = ReadPublicKey(publicKeyPath);
                SecretKey = ReadSecretKey(privateKeyPath);
                PrivateKey = ReadPrivateKey(passPhrase);
            }

            #region Secret Key

            private PgpSecretKey ReadSecretKey(string privateKeyPath)
            {
                using (Stream keyIn = File.OpenRead(privateKeyPath))
                {
                    using (Stream inputStream = PgpUtilities.GetDecoderStream(keyIn))
                    {
                        PgpSecretKeyRingBundle secretKeyRingBundle = new PgpSecretKeyRingBundle(inputStream);
                        PgpSecretKey foundKey = GetFirstSecretKey(secretKeyRingBundle);
                        if (foundKey != null)
                        {
                            return foundKey;
                        }
                    }
                }

                throw new ArgumentException("Can't find signing key in key ring.");
            }

            /// <summary>
            /// Return the first key we can use to encrypt.
            /// Note: A file can contain multiple keys (stored in "key rings")
            /// </summary>
            private PgpSecretKey GetFirstSecretKey(PgpSecretKeyRingBundle secretKeyRingBundle)
            {
                foreach (PgpSecretKeyRing keyRing in secretKeyRingBundle.GetKeyRings())
                {
                    PgpSecretKey key = keyRing
                        .GetSecretKeys()
                        .Cast<PgpSecretKey>()
                        .FirstOrDefault(k => k.IsSigningKey);

                    if (key != null)
                    {
                        return key;
                    }
                }

                return null;
            }

            #endregion Secret Key

            #region Public Key

            private PgpPublicKey ReadPublicKey(string publicKeyPath)
            {
                using (Stream keyIn = File.OpenRead(publicKeyPath))
                {
                    using (Stream inputStream = PgpUtilities.GetDecoderStream(keyIn))
                    {
                        PgpPublicKeyRingBundle publicKeyRingBundle = new PgpPublicKeyRingBundle(inputStream);
                        PgpPublicKey foundKey = GetFirstPublicKey(publicKeyRingBundle);
                        if (foundKey != null)
                        {
                            return foundKey;
                        }
                    }
                }

                throw new ArgumentException("No encryption key found in public key ring.");
            }

            private PgpPublicKey GetFirstPublicKey(PgpPublicKeyRingBundle publicKeyRingBundle)
            {
                foreach (PgpPublicKeyRing keyRing in publicKeyRingBundle.GetKeyRings())
                {
                    PgpPublicKey key = keyRing
                        .GetPublicKeys()
                        .Cast<PgpPublicKey>()
                        .FirstOrDefault(k => k.IsEncryptionKey);

                    if (key != null)
                    {
                        return key;
                    }
                }

                return null;
            }

            #endregion Public Key

            #region Private Key

            private PgpPrivateKey ReadPrivateKey(string passPhrase)
            {
                PgpPrivateKey privateKey = SecretKey.ExtractPrivateKey(passPhrase.ToCharArray());
                if (privateKey == null)
                {
                    throw new ArgumentException("No private key found in secret key.");
                }

                return privateKey;
            }

            #endregion Private Key
        }
    }
}