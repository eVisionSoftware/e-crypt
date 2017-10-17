namespace eVision.eCrypt.KeyGenerator.Helpers.Validation
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    internal class DataAnnotationsValidator
    {
        private readonly object target;
        public DataAnnotationsValidator(object target)
        {
            this.target = target;
        }

        public ICollection<ValidationResult> Validate()
        {
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(target, null, null);
            return !Validator.TryValidateObject(target, validationContext, validationResults, true) ? validationResults
                                                                                                    : new List<ValidationResult>();
        }

        public ICollection<string> ValidateProperty(string propertyName)
        {
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            object value = target.GetType().GetProperty(propertyName).GetValue(target);
            ValidationContext validationContext = new ValidationContext(target, null, null) { MemberName = propertyName };

            return !Validator.TryValidateProperty(value, validationContext, validationResults)
                ? validationResults.Select(e => e.ErrorMessage).ToList()
                : new List<string>();
        }
    }
}
