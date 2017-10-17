namespace eVision.eCrypt.KeyGenerator.Helpers.Validation
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public class ValidateByMemberAttribute : ValidationAttribute
    {
        public readonly string ValidationMember;

        public ValidateByMemberAttribute(string validationMember)
        {
            ValidationMember = validationMember;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool isValid = false;
            string message = Resources.Resources.ResourceManager.GetString(ErrorMessageResourceName);
            MemberInfo member = validationContext.ObjectType.GetMember(ValidationMember).Single();
            switch (member.MemberType)
            {
                case MemberTypes.Method:
                {
                        var method = validationContext.ObjectType.GetMethod(ValidationMember);
                        isValid = (bool)method.Invoke(validationContext.ObjectInstance, new[] { value });
                        break;
                }
                case MemberTypes.Property:
                {
                        var property = validationContext.ObjectType.GetProperty(ValidationMember);
                        isValid = (bool)property.GetValue(validationContext.ObjectInstance);
                        break;
                }
            }

            return isValid ? null : new ValidationResult(message, new [] { validationContext.MemberName });
        }
    }
}
