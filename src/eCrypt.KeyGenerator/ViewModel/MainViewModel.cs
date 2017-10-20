namespace eVision.eCrypt.KeyGenerator.ViewModel
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using GalaSoft.MvvmLight.CommandWpf;
    using Prism.Events;
    using eCrypt.Common;
    using eVision.eCrypt.KeyGenerator.Helpers;
    using eVision.eCrypt.KeyGenerator.Resources;
    using eVision.eCrypt.KeyGenerator.ViewModel.Steps;
    using eVision.Encryption;

    internal class MainViewModel : ViewModel
    {
        public MainViewModel(IEventAggregator eventAggregator,
                             ActionStepViewModel actionStep,
                             KeyGenerationStepViewModel keyGenerationStep)
            :base(eventAggregator)
        {
            Application.Current.DispatcherUnhandledException += (s,a)=> OnException(a.Exception);
            FinishCommand = new RelayCommand(Finish);
            ActionStep = actionStep;
            KeyGenerationStep = keyGenerationStep;
      
            InitializeValidation();
        }

        public ActionStepViewModel ActionStep { get; }

        public KeyGenerationStepViewModel KeyGenerationStep { get; }

        public RelayCommand FinishCommand { get; }

        bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; RaiseEvents(); }
        }

        public string GetNextStepName(string baseStepName) => nameof(KeyGenerationStep);
        
        private async void Finish()
        {
            if (!Validate())
            {
                return;
            }

            IsBusy = true;
           
            if (await GenerateKeys())
            {
                MessageBoxResult result = MessageBox.Show(string.Format(Resources.Finish_KeysGenerated, KeyGenerationStep.DestinationFolder), Resources.Validation_Success, MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                if (result == MessageBoxResult.Yes)
                {
                    Process.Start("explorer.exe", KeyGenerationStep.DestinationFolder);
                }
                Application.Current.Shutdown();
            }
            IsBusy = false;
        }

        private async Task<bool> GenerateKeys()
        {
            string publicKeyPath = KeyGenerationStep.PublicKeyPath,
                   privateKeyPath = KeyGenerationStep.PrivateKeyPath;

            if (File.Exists(privateKeyPath) || File.Exists(publicKeyPath))
            {
                MessageBoxResult overwriteResult = CustomMessageBox.Show(Resources.KeyGenerationStep_DoYouWantOverwrite,
                    noText: Resources.KeyGenerationStep_NoSelectAnother);
                if (overwriteResult == MessageBoxResult.No)
                {
                    return false;
                }
            }

            await Task.Factory.StartNew(() =>
            {
                PgpKeyGenerator.GenerateKey(DefaultCredentials.Username, DefaultCredentials.Password, publicKeyPath, privateKeyPath);
            });

            return true;
        }
        
        private void OnException(Exception ex)
        {
            IsBusy = true;
            if (ex is ApplicationException)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                IsBusy = false;
            }
        }

        public override bool Validate()
        {
            return KeyGenerationStep.Validate();
        }
    }
}