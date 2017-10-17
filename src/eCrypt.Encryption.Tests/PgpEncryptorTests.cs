namespace eVision.Encryption.Tests
{
    using System.CodeDom.Compiler;
    using FluentAssertions;
    using Xunit;
    using static System.IO.File;
    using Utils;
    using Encryption;

    public class PgpEncryptorTests
    {
        [Fact]
        public void Can_encrypt_file()
        {
            using (var files = new TempFileCollection())
            {
                string inputFilePath = files.AddFile();
                string outputFilePath = files.AddFile();
                WriteAllText(inputFilePath, "Hello Encryption");

                PgpEncryptor.EncryptFile(inputFilePath, outputFilePath, TestKeys.PublicKey);

                outputFilePath.Should().BeExistingFilePath();
            }
        }
    }
}
