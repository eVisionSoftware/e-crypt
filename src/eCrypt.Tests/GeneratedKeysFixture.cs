using System.IO;

namespace eVision.eCrypt.Tests
{
    using System;
    using System.CodeDom.Compiler;
    using Encryption;
    using Encryption.Tests.Utils;

    public class GeneratedKeysFixture : IDisposable
    {
        private readonly TempFileCollection files = new TempFileCollection();

        public GeneratedKeysFixture()
        {
            PublicKeyPath = files.AddFile();
            PrivateKeyPath = files.AddFile();
            PgpKeyGenerator.GenerateKey(TestKeys.Username, TestKeys.Password, PublicKeyPath, PrivateKeyPath);
            PublicKey = File.ReadAllText(PublicKeyPath);
        }

        public string PublicKeyPath { get; }
        public string PrivateKeyPath { get; }
        public string PublicKey { get; }

        public void Dispose()
        {
            files.Delete();
        }
    }
}
