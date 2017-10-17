namespace eVision.eCrypt.KeyGenerator.ViewModel.Steps
{
    using Prism.Events;
    using Helpers;

    internal class ActionStepViewModel : ViewModel
    {
        public ActionStepViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            InitializeValidation();
        }
    }
}
