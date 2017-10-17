namespace eVision.eCrypt.Tests
{
    using System;
    using System.IO.Compression;
    using System.Linq;
    using System.CodeDom.Compiler;
    using System.Threading.Tasks;
    using Xunit;
    using FluentAssertions;
    using Encryption.Tests.Utils;
    using Runners;
    using Encryption.Helpers;
    using Common;
    using Helpers.Extensions;

    public class ProgramTests : IClassFixture<GeneratedKeysFixture>
    {
        private readonly GeneratedKeysFixture Keys;

        public ProgramTests(GeneratedKeysFixture generatedKeys)
        {
            Keys = generatedKeys;
        }

        [Fact]
        public async Task When_package_is_directory_can_generate_executable_and_decrypt()
        {
            using (var sourcePackageDirectory = TempDirectory.InBaseDirectory())
            using (var decryptedPackageDirectory = TempDirectory.InBaseDirectory())
            using (var files = new TempFileCollection(AppDomain.CurrentDomain.BaseDirectory))
            {
                string selfExtractorPath = files.AddFileWithExtension("exe");
                string encryptedContent = $"Encrypted content {Guid.NewGuid()}";
                sourcePackageDirectory.CreateTextFile("file.txt", encryptedContent);
                files.AddFile("decryption.log");

                ProcessResult encryptionResult = await new eCryptRunner()
                    .RunAsync(selfExtractorPath, Keys.PublicKeyPath, sourcePackageDirectory.Path);
                ProcessResult decryptionResult = await new SelfExtractorRunner(selfExtractorPath)
                    .RunAsync(Keys.PrivateKeyPath, decryptedPackageDirectory.Path);

                encryptionResult.Should().BeSuccessfullProcessResult();
                decryptionResult.Should().BeSuccessfullProcessResult();
                decryptedPackageDirectory.Files.Count().Should().Be(1);
                decryptedPackageDirectory.Files.Single().Should().BePathToFileWithContent(encryptedContent);
            }
        }

        [Fact]
        public async Task When_package_is_file_can_generate_executable_and_decrypt()
        {
            using (var sourcePackageDirectory = TempDirectory.InBaseDirectory())
            using (var decryptedPackageDirectory = TempDirectory.InBaseDirectory())
            using (var files = new TempFileCollection(AppDomain.CurrentDomain.BaseDirectory))
            {
                string selfExtractorPath = files.AddFileWithExtension("exe");
                string encryptedContent = $"Encrypted content {Guid.NewGuid()}";
                sourcePackageDirectory.CreateTextFile("file.txt", encryptedContent);
                files.AddFile("decryption.log");

                string archivedPackage = files.AddFileWithExtension("zip");
                ZipFile.CreateFromDirectory(sourcePackageDirectory.Path, archivedPackage, CompressionLevel.NoCompression, false);

                ProcessResult encryptionResult = await new eCryptRunner()
                    .RunAsync(selfExtractorPath, Keys.PublicKeyPath, archivedPackage);
                ProcessResult decryptionResult = await new SelfExtractorRunner(selfExtractorPath)
                    .RunAsync(Keys.PrivateKeyPath, decryptedPackageDirectory.Path);

                encryptionResult.Should().BeSuccessfullProcessResult();
                decryptionResult.Should().BeSuccessfullProcessResult();
                decryptedPackageDirectory.Files.Count().Should().Be(1);
                decryptedPackageDirectory.Files.Single().Should().BePathToFileWithContent(encryptedContent);
            }
        }

        [Fact]
        public async Task When_public_key_as_a_parameter_it_can_generate_executable_and_decrypt()
        {
            using (var sourcePackageDirectory = TempDirectory.InBaseDirectory())
            using (var decryptedPackageDirectory = TempDirectory.InBaseDirectory())
            using (var files = new TempFileCollection(AppDomain.CurrentDomain.BaseDirectory))
            {
                string selfExtractorPath = files.AddFileWithExtension("exe");
                string encryptedContent = $"Encrypted content {Guid.NewGuid()}";
                sourcePackageDirectory.CreateTextFile("file.txt", encryptedContent);
                files.AddFile("decryption.log");

                string archivedPackage = files.AddFileWithExtension("zip");
                ZipFile.CreateFromDirectory(sourcePackageDirectory.Path, archivedPackage, CompressionLevel.NoCompression, false);

                ProcessResult encryptionResult = await new eCryptRunner()
                    .RunAsyncWithPublicKey(selfExtractorPath, Keys.PublicKey, archivedPackage);
                ProcessResult decryptionResult = await new SelfExtractorRunner(selfExtractorPath)
                    .RunAsync(Keys.PrivateKeyPath, decryptedPackageDirectory.Path);

                encryptionResult.Should().BeSuccessfullProcessResult();
                decryptionResult.Should().BeSuccessfullProcessResult();
                decryptedPackageDirectory.Files.Count().Should().Be(1);
                decryptedPackageDirectory.Files.Single().Should().BePathToFileWithContent(encryptedContent);
            }
        }
    }
}
