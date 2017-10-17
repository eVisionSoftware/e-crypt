namespace eVision.eCrypt.KeyGenerator
{
    using System.Windows;
    using Microsoft.Practices.ServiceLocation;
    using Prism.Events;
    using Xceed.Wpf.Toolkit;
    using Xceed.Wpf.Toolkit.Core;
    using ViewModel;

    public partial class MainWindow : Window
    {
        private static IEventAggregator EventAggregator => ServiceLocator.Current.GetInstance<IEventAggregator>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Wizard_OnNext(object sender, CancelRoutedEventArgs e)
        {
            var wizard = ((Wizard)sender);
            string stepName = wizard.CurrentPage.Name;
            var vm = (MainViewModel)Window.DataContext;
            var nextPage = (WizardPage)Window.FindName(vm.GetNextStepName(stepName));

            e.Cancel = true;
            wizard.CurrentPage = nextPage;
        }
    }
}
