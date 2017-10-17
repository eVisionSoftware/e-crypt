namespace eVision.eCrypt.KeyGenerator.ViewModel
{
    using GalaSoft.MvvmLight.Ioc;
    using Microsoft.Practices.ServiceLocation;
    using Prism.Events;
    using Steps;

    internal class ViewModelLocator
    {
        private readonly SimpleIoc Ioc = SimpleIoc.Default;
        private readonly IEventAggregator eventAggregator = new EventAggregator();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => Ioc);
            Ioc.Register<MainViewModel>();
            Ioc.Register<KeyGenerationStepViewModel>();
            Ioc.Register<ActionStepViewModel>();

            Ioc.Unregister<IEventAggregator>();
            Ioc.Register<IEventAggregator>(()=> eventAggregator);
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static void Cleanup()
        {
        }
    }
}