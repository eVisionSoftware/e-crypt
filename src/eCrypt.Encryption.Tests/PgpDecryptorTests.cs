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

    public class PgpDecryptorTests
    {
        [Fact]
        public void Can_decrypt_file()
        {
            using (var files = new TempFileCollection())
            {
                string inputFilePath = files.AddFile();
                string encryptedFilePath = files.AddFile();
                string decryptedFilePath = files.AddFile();
                string content = $"Hello Decryption {Guid.NewGuid()}";
                WriteAllText(inputFilePath, content);
                PgpEncryptor.EncryptFile(inputFilePath, encryptedFilePath, TestKeys.PublicKey);

                using (FileStream encryptedStream = OpenRead(encryptedFilePath))
                using (FileStream privateKeyStream = OpenRead(TestKeys.PrivateKeyPath))
                {
                    PgpDecryptor.Decrypt(encryptedStream, privateKeyStream, TestKeys.Password, decryptedFilePath);
                }

                decryptedFilePath.Should().BePathToFileWithContent(content);
            }
        }
    }
}
