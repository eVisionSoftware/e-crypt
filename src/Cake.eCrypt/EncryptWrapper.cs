namespace Cake.eCrypt
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;

    using eVision.eCrypt;
    using eVision.eCrypt.Common;
    using eVision.eCrypt.Transformations;

    internal class EncryptWrapper
    {
        private readonly Log logInformational;
        private readonly Log logError;

        public delegate void Log(string message, params string[] args);

        public EncryptWrapper(Log info, Log error)
        {
            logInformational = info;
            logError = error;
        }

        internal void Encrypt(string target, string keyPath, string outputPath)
        {
            try
            {
                EmbeddedAssemblyResolver.Init((name) => { }, (name) => { });
                string publicKey = GetPublicKey(keyPath);

                using (var tempFiles = new TempFileCollection())
                {
                    using (var compiler = new SelfExtractorCompiler(new EncryptFileTransformation(publicKey))
                        .IncludingFiles(target))
                    {
                        string path = compiler.Compile(outputPath);
                        logInformational($"Self extractable executable was generated at {path} using key {publicKey}");
                    }
                }
            }
            catch (Exception ex)
            {
                logError($"Error during exncryption: {ex.Message}");
            }
        }

        private static string GetPublicKey(string keyPath)
        {
            return File.ReadAllText(keyPath);
        }
    }
}
