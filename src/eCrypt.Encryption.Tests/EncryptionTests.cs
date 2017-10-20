namespace eVision.Encryption.Tests
{
    using System;
    using System.IO;
    using System.CodeDom.Compiler;
    using FluentAssertions;
    using Xunit;
    using static System.IO.File;
    using Utils;
    using Encryption;

    public class EncryptionTests
    {
        [Fact]
        public void Can_generate_keys_encrypt_and_decrypt_file()
        {
            using (var files = new TempFileCollection())
            {
                string publicKeyPath = files.AddFile();
                string privateKeyPath = files.AddFile();
                string inputFilePath = files.AddFile();
                string encryptedFilePath = files.AddFile();
                string decryptedFilePath = files.AddFile();
                string content = $"Hello Decryption {Guid.NewGuid()}";
                WriteAllText(inputFilePath, content);
                PgpKeyGenerator.GenerateKey(TestKeys.Username, TestKeys.Password, publicKeyPath, privateKeyPath);
                PgpEncryptor.EncryptFile(inputFilePath, encryptedFilePath, ReadAllText(publicKeyPath));

                using (FileStream encryptedStream = OpenRead(encryptedFilePath))
                using (FileStream privateKeyStream = OpenRead(privateKeyPath))
                {
                    PgpDecryptor.Decrypt(encryptedStream, privateKeyStream, TestKeys.Password, decryptedFilePath);
                }

                decryptedFilePath.Should().BePathToFileWithContent(content);
            }
        }
    }
}
