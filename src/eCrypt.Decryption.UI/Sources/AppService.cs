namespace eVision.Decryption.UI
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Windows.Forms;
    using Encryption;
    using eCrypt.Common;
    
    public class AppService
    {
        private const string DebugResourcePrefix = "eVision.Decryption.UI.Resources.";
        private const string DebugResource = DebugResourcePrefix + "encryptedForDebug";
        private const string EncryptedExtensions = "pgp";
        private const string ArchiveName = "DecryptedArchive.zip";
        private static readonly string passPhrase = DefaultCredentials.Password;
        private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        private readonly Options options;
        private static readonly string DecryptedFolderName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);

        public AppService(Options options)
        {
            this.options = options;
        }

        public async Task ExtractAsync(string privateKey, string destinationDirectory)
        {
            await Task.Factory.StartNew(() => { Extract(privateKey, destinationDirectory); });
        }

        public void Extract(string privateKey, string destinationDirectory)
        {
            string decryptedArchivePath = Path.Combine(destinationDirectory, ArchiveName);

            if (File.Exists(decryptedArchivePath))
            {
                throw new ApplicationException(string.Format("File already exists {0}", decryptedArchivePath));
            }

            try
            {
                Decrypt(privateKey, decryptedArchivePath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                throw new ApplicationException("Could not decrypt package: Check if a private key is correct");
            }

            string decryptedDir = GetDecryptedDir(destinationDirectory);
            if (Directory.Exists(decryptedDir))
            {
                throw new ApplicationException(string.Format("Destination directory already exists:{0}", DecryptedFolderName));
            }

            try
            {
                Unarchive(decryptedArchivePath, decryptedDir);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Package was decrypted to archive '{0}'. But it couldn't be unziped. {1}", decryptedArchivePath, ex.Message));
            }

            if (File.Exists(decryptedArchivePath))
            {
                File.Delete(decryptedArchivePath);
            }
        }

        private void Unarchive(string archivePath, string decryptedDir)
        {
            Directory.CreateDirectory(decryptedDir);
            ZipFile.ExtractToDirectory(archivePath, decryptedDir);
        }

        public string GetDecryptedDir(string destinationDirectory)
        {
            return Path.Combine(destinationDirectory,
                           Path.GetFileNameWithoutExtension(Application.ExecutablePath));
        }

        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        public Stream GetResource(string resourceKey)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();
            string fullKey = options.IsDevelopment ? DebugResourcePrefix + resourceKey : resourceKey;

            if (resourceNames.Contains(fullKey))
            {
                return assembly.GetManifestResourceStream(fullKey);
            }

            string message = string.Format("Could not load resource. Have: {0}", string.Join(",", resourceNames));
            Logger.Error(message);
            throw new ApplicationException(message);
        }

        private void Decrypt(string privateKey, string outputFile)
        {
            string encryptedResourceKey = FindEncryptedResource();
            if (encryptedResourceKey == null)
            {
                Logger.Error(string.Format("Could not find encrypted resourse. Assembly contains following resources: {0}", string.Join(",", ExecutingAssembly.GetManifestResourceNames())));
                throw new ApplicationException("Missing embedded package");
            }

            Logger.Debug(string.Format("Encrypted data is stored as resource with the key {0}", encryptedResourceKey));
            using (Stream inputStream = ExecutingAssembly.GetManifestResourceStream(encryptedResourceKey))
            using (Stream privateKeyStream = privateKey.OpenStream())
            {
                PgpDecryptor.Decrypt(inputStream, privateKeyStream, passPhrase, outputFile);
            }
        }

        private string FindEncryptedResource()
        {
            return options.IsDevelopment
                ? DebugResource
                : ExecutingAssembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith(EncryptedExtensions));
        }
    }
}
