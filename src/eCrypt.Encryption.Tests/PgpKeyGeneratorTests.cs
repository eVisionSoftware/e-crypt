namespace eVision.Encryption.Tests
{
    using System.CodeDom.Compiler;
    using FluentAssertions;
    using Xunit;
    using Encryption;
    using Utils;
    using eCrypt.Common;

    public class PgpKeyGeneratorTests
    {
        [Fact]
        public void Can_generate_keys()
        {
            using (var files = new TempFileCollection())
            {
                string publicKeyPath = files.AddFile();
                string privateKeyPath = files.AddFile();

                PgpKeyGenerator.GenerateKey(DefaultCredentials.Username, DefaultCredentials.Password, publicKeyPath, privateKeyPath);

                publicKeyPath.Should().BeValidPublicKeyFilePath();
                privateKeyPath.Should().BeValidPrivateKeyFilePath();
            }
        }
    }
}
