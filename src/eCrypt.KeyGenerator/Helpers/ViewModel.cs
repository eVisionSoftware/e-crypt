namespace eVision.eCrypt.KeyGenerator.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Extensions;
    using Validation;

    using GalaSoft.MvvmLight;

    using Prism.Events;

    internal class ViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly DataAnnotationsValidator _validator;
        private readonly Dictionary<string, ICollection<string>> _validationErrors = new Dictionary<string, ICollection<string>>();
        private bool _isInitialized;

        protected readonly IEventAggregator EventAggregator;

        public ViewModel(IEventAggregator eventAggregator)
        {
            _validator = new DataAnnotationsValidator(this);
            EventAggregator = eventAggregator;
        }

        public string Name => GetType().Name.TrimSuffix(nameof(ViewModel));

        public bool IsValid => !HasErrors;

        public virtual bool Validate()
        {
            var errors = _validator.Validate();
            foreach (var propertyName in errors.SelectMany(e => e.MemberNames).Distinct())
            {
                var propertyErros = errors.Where(e => e.MemberNames.Contains(propertyName)).Select(e => e.ErrorMessage).ToList();
                SetPropertyErrors(propertyName, propertyErros);
            }

            return !errors.Any();
        }

        protected void InitializeValidation() => _isInitialized = true;

        protected void RaiseEvents([CallerMemberName]string propertyName = null)
        {
            SetPropertyErrors(propertyName, _validator.ValidateProperty(propertyName));
            RaisePropertyChanged(propertyName);
        }

        protected void SetPropertyErrors(string propertyName, ICollection<string> errors)
        {
            if (!_isInitialized)
            {
                return;
            }

            if (_validationErrors.ContainsKey(propertyName))
            {
                _validationErrors.Remove(propertyName);
            }

            if (errors.Any())
            {
                _validationErrors.Add(propertyName, errors);
            }

            RaiseErrorsChanged(propertyName);
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            RaisePropertyChanged(nameof(HasErrors));
            RaisePropertyChanged(nameof(IsValid));
        }

        #region INotifyDataErrorInfo
        public virtual IEnumerable GetErrors(string propertyName)
            => _validationErrors.ContainsKey(propertyName) ? _validationErrors[propertyName] : null;

        public virtual bool HasErrors => _validationErrors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion
    }
}
