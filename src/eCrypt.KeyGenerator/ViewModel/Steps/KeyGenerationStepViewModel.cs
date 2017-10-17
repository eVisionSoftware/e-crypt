namespace eVision.eCrypt.KeyGenerator.ViewModel.Steps
{
    using System.IO;
    using System.Reflection;
    using Prism.Events;
    using System.ComponentModel.DataAnnotations;
    using Helpers;
    using Helpers.Validation;
    using Properties;

    internal class KeyGenerationStepViewModel : ViewModel
    {
        private const string KeyFileExtension = ".asc";
        private const string PublicKeyFileName = "public key" + KeyFileExtension;
        private const string PrivateKeyFileName = "private key" + KeyFileExtension;

        public KeyGenerationStepViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            DestinationFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            InitializeValidation();
        }

        private void OnDestinationChanged()
        {
            PublicKeyPath = Path.Combine(DestinationFolder, PublicKeyFileName);
            PrivateKeyPath = Path.Combine(DestinationFolder, PrivateKeyFileName);
        }

        string _destinationFolder;
        [Required]
        [ExistingPath(IsDirectory = true, ErrorMessageResourceName = "Validation_ExisingPath", ErrorMessageResourceType = typeof(Resources))]
        //TODO: For some reason tooltip is not shown. For now existing keys will be checked in MainViewModel
        //[ValidateByMember(nameof(DestinationFolderHasNoExistingKeys), ErrorMessageResourceName = "Validation_AlreadyHasKey", ErrorMessageResourceType = typeof(Resources))]
        public string DestinationFolder
        {
            get { return _destinationFolder; }
            set { _destinationFolder = value; RaiseEvents(); OnDestinationChanged(); }
        }

        public bool DestinationFolderHasNoExistingKeys
            => !(!string.IsNullOrWhiteSpace(DestinationFolder) && (File.Exists(PublicKeyPath) || File.Exists(PrivateKeyPath)));

        string _publicKeyPath;
        public string PublicKeyPath
        {
            get { return _publicKeyPath; }
            set { _publicKeyPath = value; RaiseEvents(); }
        }

        string _privateKeyPath;
        public string PrivateKeyPath
        {
            get { return _privateKeyPath; }
            set { _privateKeyPath = value; RaiseEvents(); }
        }
    }
}
