namespace eVision.Encryption.Tests.Utils
{
    using eCrypt.Common;
    using FluentAssertions;
    using FluentAssertions.Primitives;
    using static System.IO.File;

    public static class StringAssertionsExtensions
    {
        public static void BeValidPublicKeyFilePath(this StringAssertions assertions)
        {
            BeExistingFilePath(assertions);
            ReadAllText(assertions.Subject).Should().Match(text => PGPValidator.IsPublicKey(text));
        }

        public static void BeValidPrivateKeyFilePath(this StringAssertions assertions)
        {
            BeExistingFilePath(assertions);
            ReadAllText(assertions.Subject).Should().Match(text => PGPValidator.IsPrivateKey(text));
        }

        public static void BePathToFileWithContent(this StringAssertions assertions, string content)
        {
            BeExistingFilePath(assertions);
            ReadAllText(assertions.Subject).Should().Be(content);
        }

        public static void BeExistingFilePath(this StringAssertions assertions)
        {
            Exists(assertions.Subject).Should().BeTrue();
        }
    }
}
