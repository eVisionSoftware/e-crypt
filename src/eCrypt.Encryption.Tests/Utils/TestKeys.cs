using System;
using System.IO;

namespace eVision.Encryption.Tests.Utils
{
    using eCrypt.Common;
    using static System.IO.Path;
    using static System.IO.Directory;

    public static class TestKeys
    {
        private static readonly string Directory = Combine(GetCurrentDirectory(), "Resources");
        public static string Username { get; } = DefaultCredentials.Username;
        public static string Password { get; } = DefaultCredentials.Password;
        public static string PublicKeyPath { get; } = Combine(Directory, "pub.asc");
        public static string PublicKey { get; } = File.Exists(PublicKeyPath) ? File.ReadAllText(PublicKeyPath): String.Empty;
        public static string PrivateKeyPath { get; } = Combine(Directory, "sec.asc");
    }
}
