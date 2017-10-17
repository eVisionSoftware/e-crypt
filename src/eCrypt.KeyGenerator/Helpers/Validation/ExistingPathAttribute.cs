namespace eVision.eCrypt.KeyGenerator.Helpers.Validation
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;

    internal class ExistingPathAttribute : ValidationAttribute
    {
        public bool IsDirectory;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string path = (string)value;
            string message = Resources.Resources.ResourceManager.GetString(ErrorMessageResourceName);
            bool isValid = string.IsNullOrEmpty(path) || (IsDirectory ? Directory.Exists(path) : File.Exists(path));
            return isValid ? null : new ValidationResult(message, new[] { validationContext.MemberName });
        }
    }
}
